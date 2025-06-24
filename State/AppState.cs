using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using Vocab.Api;
using Vocab.Models;
using Vocab.Pages;

namespace Vocab.State
{
    public class AppState
    {
        private readonly GoogleSheet _googleSheet;
        private readonly IJSRuntime _js;

        public AppState(GoogleSheet googleSheet, IJSRuntime jsRuntime)
        {
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
                var storedSheetId = await _js.InvokeAsync<string>("Web.getStorageItemAsString", "sheet-id");
                var storageTranslations = await _js.InvokeAsync<List<TranslationItem>>("Web.getStorageItem", "translations");
                var incorrectGuesses = await _js.InvokeAsync<int>("Web.getStorageItem", "incorrectGuesses");

                if (!string.IsNullOrEmpty(storedSheetId))
                {
                    await SetSheetId(storedSheetId); //This also gets translations
                }
                if (storageTranslations != null)
                {
                    Translations = storageTranslations;
                    IncorrectGuesses = incorrectGuesses; 
                    NotifyTranslationsLoaded();     
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
            Console.WriteLine("set sheet id");
            Translations = await _googleSheet.GetEntries(SheetId!);
            await _js.InvokeVoidAsync("Web.saveToStorage", "translations", JsonConvert.SerializeObject(Translations));
            NotifySheetIdChanged();
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

        public async Task UpdateCorrectGuess(int index)
        {
            Console.WriteLine("update guess");
            Translations[index].IsGuessed = true;
            await _js.InvokeVoidAsync("Web.saveToStorage", "translations", JsonConvert.SerializeObject(Translations));
            NotifyStateChanged();
        }
        public async Task UpdateIncorrectGuesses()
        {
            IncorrectGuesses++;
            await _js.InvokeVoidAsync("Web.saveToStorage", "incorrectGuesses", IncorrectGuesses);
            NotifyStateChanged();
        }

        public List<TranslationMultipleChoices> GetMultipleChoiceSets(int count)
        {
            Random random = new Random();
            var cardDataSet = Translations
                .Where(w => !w.IsGuessed)
                .OrderBy(x => random.Next())
                .Take(count >= Translations.Count ? Translations.Count : count)
                .ToList()
                .Select(s => { return CreateCardMultipleChoices(s, 2); })
                .ToList();

            NotifyStateChanged();
            return cardDataSet;
        }

        public TranslationMultipleChoices? GetNextMultipleChoiceSet(int minimumVisible)
        {
            //This might be running before the isGuessed is properfly finished settings
            var remaining = Translations.Where(w => !w.IsGuessed).ToList();
            Console.WriteLine($"Remaining: {remaining.Count}, Visible: {minimumVisible}");


            //This is replacing the 2nd to last one with the same as the final
            //need to still 
            // maybe  will need to check how many true
            //if (remaining.Count == 1)
            //{
            //    return CreateCardMultipleChoices(remaining.First(), 2);
            //}

            if (remaining.Count < minimumVisible)
            {
                return null;
            }
            return GetMultipleChoiceSets(1).FirstOrDefault();
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

        public async Task ReSetGuesses()
        {
            Console.WriteLine("resetguesses");
            Translations.ForEach(fe => { fe.IsGuessed = false; });
            await _js.InvokeVoidAsync("Web.saveToStorage", "translations", JsonConvert.SerializeObject(Translations));
            await _js.InvokeVoidAsync("Web.saveToStorage", "incorrectGuesses", "0");
            IncorrectGuesses = 0;
            NotifyReStart();
        
        }

        public void Shuffle()
        {
            NotifyReStart();
        }

        public async Task Reset()
        {
            await _js.InvokeVoidAsync("Web.clearStorage");
            Translations = new List<TranslationItem>();
            SheetId = null;
            IncorrectGuesses = 0;
            NotifyStateChanged();
            NotifySheetIdChanged();
        }
    }
}
