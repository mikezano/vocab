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
                Console.WriteLine("initialize");
                var storedSheetId = await _js.InvokeAsync<string>("Web.getStorageItemAsString", "sheet-id-prev");
                var storageTranslations = await _js.InvokeAsync<List<TranslationItem>>("Web.getStorageItem", "translations");

                if (!string.IsNullOrEmpty(storedSheetId))
                {
                    await SetSheetId(storedSheetId); //This also gets translations
                }
                if (storageTranslations != null)
                {
                    Translations = storageTranslations;
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

        public void UpdateCorrectGuess(int index)
        {
            Console.WriteLine("update guess");
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

        public TranslationMultipleChoices? GetNextMultipleChoiceSet(int minimumVisible)
        {
            var remaining = Translations.Where(w => !w.IsGuessed).ToList();
            if(remaining.Count < minimumVisible)
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

        public void ReSetGuesses()
        {
            Console.WriteLine("resetguesses");
            Translations.ForEach(fe => { fe.IsGuessed = false; });
            _js.InvokeVoidAsync("Web.saveToStorage", "translations", JsonConvert.SerializeObject(Translations));
            NotifyReStart();
            IncorrectGuesses = 0;
        }

        public void Shuffle()
        {
            NotifyReStart();
        }

        public async Task Reset()
        {
            await _js.InvokeVoidAsync("Web.clearStorageItem", "translations");
            await _js.InvokeVoidAsync("Web.clearStorageItem", "sheet-id");
            await _js.InvokeVoidAsync("Web.clearStorageItem", "sheet-id-prev");
            Translations = new List<TranslationItem>();
            SheetId = null;
            NotifyStateChanged();
            NotifySheetIdChanged();
        }
    }
}
