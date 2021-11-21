using System;
using Web.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Web.Api;
using Newtonsoft.Json;

namespace Web.Shared
{
    public partial class Cards
    {
        public List<TranslationMultipleChoices> MultipleChoiceSets { get; set; } = new List<TranslationMultipleChoices>();

        [Inject]
        public AppState AppState { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; }
        [Inject]
        public GoogleSheet GoogleSheet { get; set; }

        private static Random random = new Random();
        private List<TranslationItem> _translations = new List<TranslationItem>();
        private int _visibleCardCount = 0;

        protected override async Task OnInitializedAsync()
        {
            var storageTranslations = await JS.InvokeAsync<List<TranslationItem>>("Web.getStorageItem", "translations");

            if (storageTranslations == null)
            {
                _translations = await GoogleSheet.GetEntries(AppState.SheetId);
                await JS.InvokeVoidAsync("Web.saveToStorage", "translations", JsonConvert.SerializeObject(_translations));
            }
            else
            {
                _translations = storageTranslations;
            }

            AppState.SetWords(_translations.ToList());

            var dimensions = await JS.InvokeAsync<BrowserDimensions>("Web.getDimensions");
            _visibleCardCount = dimensions.GetVisibleCardCount();

            MultipleChoiceSets = AppState.GetMultipleChoiceSets(_visibleCardCount);
        }

        public void ReStart()
        {
            MultipleChoiceSets = AppState.GetMultipleChoiceSets(_visibleCardCount);
            StateHasChanged();
        }

        protected override void OnInitialized()
        {
            AppState.OnReStart += ReStart;
        }

        public void Dispose()
        {
            AppState.OnReStart -= ReStart;
        }

        public void OnCorrect(Answer answer)
        {
            int indexOfWord = AppState.Translations.FindIndex(f => f.From == answer.Translation);
            if (answer.IsCorrect)
            {
                AppState.UpdateCorrectGuess(indexOfWord);
            }
            else
            {
                AppState.UpdateIncorrectGuesses();
            }

            //Next word has to be not guessed and not on screen
            //var notGuessed = AppState.Words.Where(w => !w.IsGuessed && Data.FindIndex(f => f.Translation == w.Spanish) == -1).ToList();
            var notGuessed = AppState.Translations.Where(w => !w.IsGuessed).ToList();
            var notOnScreen = notGuessed.Where(w => MultipleChoiceSets.FindIndex(fi => fi.Translation == w.From) == -1).ToList();
            if (notOnScreen.Count == 0 && !answer.IsCorrect)
            {
                notOnScreen.Add(AppState.Translations[indexOfWord]);
            }

            int nextIndex = random.Next(notOnScreen.Count);
            TranslationItem nextWord = new TranslationItem();
            if (notOnScreen.Count > 0)
            {
                nextWord = notOnScreen[nextIndex];
                MultipleChoiceSets[answer.ReplacementIndex] = AppState.CreateCardMultipleChoices(nextWord, 2);
            }
            else
            {
                MultipleChoiceSets[answer.ReplacementIndex] = new TranslationMultipleChoices { Answer = "----", Choices = null, Translation = "----"};
            }
        }
    }
}
