using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using Vocab.Api;
using Vocab.Models;

namespace Vocab.State
{
    public class AppState
    {
        private readonly GoogleSheet _googleSheet;
        private readonly IJSRuntime _js;

        public AppState(GoogleSheet googleSheet, IJSRuntime jsRuntime)
        {
            Console.WriteLine("AppState constructor called");
            _googleSheet = googleSheet ?? throw new ArgumentNullException(nameof(googleSheet));
            _js = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        }
        //[Inject]
        //public required GoogleSheet GoogleSheet { get; set; }
        public List<TranslationItem> Translations { get; private set; } = new List<TranslationItem>();
        public string? SheetId { get; private set; }
        public int IncorrectGuesses { get; set; } = 0;
        private Random _random = new Random();



        public event Action? OnChange;
        public event Action? OnSetSheetId;
        public event Action? OnReStart;
        public event Action? OnTranslationsLoaded;

        private void NotifyStateChanged() => OnChange?.Invoke();
        private void NotifySheetIdChanged() => OnSetSheetId?.Invoke();
        private void NotifyReStart() => OnReStart?.Invoke();
        private void NotifyTranslationsLoaded() => OnTranslationsLoaded?.Invoke();


        public async Task InitializeAsync()
        {
            try
            {
                var storedSheetId = await _js.InvokeAsync<string>("Web.getStorageItemAsString", "sheet-id-prev");
                Console.WriteLine($"Stored Sheet ID: {storedSheetId}");
                if (!string.IsNullOrEmpty(storedSheetId))
                {
                    await SetSheetId(storedSheetId);
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error initializing AppState: {ex.Message}");
            }   
        }

        public async Task SetSheetId(string sheetId)
        {
            SheetId = sheetId;
            NotifySheetIdChanged();
            Translations = await _googleSheet.GetEntries(SheetId);
            NotifyTranslationsLoaded();
        }


        public void SetWords(List<TranslationItem> words)
        {
            Translations = words;
            NotifyStateChanged();
        }

        public void Flip()
        {
            Translations.ForEach(t => {
                var copy = t.Clone();
                t.Word = copy.Translation;
                t.Translation = copy.Word;
            });
        }

        public int GuessedCorrectlyCount => Translations.Count(c => c.IsGuessed);

        public void UpdateCorrectGuess(int index)
        {
            Translations[index].IsGuessed = true;
            _js.InvokeVoidAsync("Web.saveToStorage", "translations", JsonConvert.SerializeObject(Translations));
            NotifyStateChanged();
        }
        public void UpdateIncorrectGuesses()
        {
            IncorrectGuesses++;
            _js.InvokeVoidAsync("Web.saveToStorage", "incorrectGuesses", IncorrectGuesses);
            NotifyStateChanged();
        }

        public List<TranslationMultipleChoices> GetMultipleChoiceSets(int count)
        {
            Random random = new Random();
            var cardDataSet = Translations
                .Where(w => !w.IsGuessed)
                .OrderBy(x => random.Next())
                .Take(count > Translations.Count ? Translations.Count : count)
                .ToList()
                .Select(s => { return CreateCardMultipleChoices(s, 2); })
                .ToList();

            NotifyStateChanged();
            return cardDataSet;
        }

        public TranslationMultipleChoices CreateCardMultipleChoices(TranslationItem item, int wrongAnswerCount)
        {
            var wrongAnswers =
                Translations
                .Where(w => w.Word != item.Word)
                .OrderBy(ob => _random.Next())
                .Select(s => s.Translation)
                .Take(wrongAnswerCount)
                .ToList();

            wrongAnswers.Add(item.Translation);
            var allAnswers = wrongAnswers
                .OrderBy(ob => _random.Next())
                .ToList();

            return new TranslationMultipleChoices 
            { 
                Choices = allAnswers, 
                Answer = item.Translation, 
                Word = item.Word 
            };
        }

        //public void GetNext()
        //{
        //    var notGuessed = Translations.Where(w => !w.IsGuessed).ToList();
        //    var notOnScreen = notGuessed.Where(w => MultipleChoiceSets.FindIndex(fi => fi.Translation == w.From) == -1).ToList();
        //    if (notOnScreen.Count == 0 && !answer.IsCorrect)
        //    {
        //        notOnScreen.Add(AppState.Translations[indexOfWord]);
        //    }

        //    int nextIndex = random.Next(notOnScreen.Count);
        //    TranslationItem nextWord = new TranslationItem();
        //    if (notOnScreen.Count > 0)
        //    {
        //        nextWord = notOnScreen[nextIndex];
        //        MultipleChoiceSets[answer.ReplacementIndex] = AppState.CreateCardMultipleChoices(nextWord, 2);
        //    }
        //    else
        //    {
        //        MultipleChoiceSets[answer.ReplacementIndex] = new TranslationMultipleChoices { Answer = "----", Choices = null, Translation = "----" };
        //    }
        //}

        public void ReSetGuesses()
        {
            Translations.ForEach(fe => { fe.IsGuessed = false; });
            _js.InvokeVoidAsync("Web.saveToStorage", "translations", JsonConvert.SerializeObject(Translations));
            NotifyReStart();
            IncorrectGuesses = 0;
        }

        public void Shuffle()
        {
            NotifyReStart();
        }

        public async void Reset()
        {
            await _js.InvokeVoidAsync("Web.clearStorageItem", "translations");
            await _js.InvokeVoidAsync("Web.clearStorageItem", "sheet-id");
            Translations = new List<TranslationItem>();
            SheetId = null;
            NotifyStateChanged();
            NotifySheetIdChanged();
        }
    }
}
