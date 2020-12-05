using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;

namespace Web
{
    public class AppState
    {
        public List<TranslationItem> Translations { get; private set; }
        public string SheetId { get; private set; } = null;
        private Random _random = new Random();

        private IJSRuntime js;

        public event Action OnChange;
        public event Action OnSetSheetId;
        public event Action OnReStart;

        private void NotifyStateChanged() => OnChange?.Invoke();
        private void NotifySheetIdChanged() => OnSetSheetId?.Invoke();
        private void NotifyReStart() => OnReStart?.Invoke();

        public void SetJsInterop(IJSRuntime js)
        {
            this.js = js;
        }

        public void SetSheetId(string sheetId)
        {
            SheetId = sheetId;
            NotifySheetIdChanged();
        }

        public void SetWords(List<TranslationItem> words)
        {
            Translations = words;
            NotifyStateChanged();
        }

        public int GuessedCorrectlyCount => Translations.Count(c => c.IsGuessed);

        public void UpdateCorrectGuess(int index)
        {
            Translations[index].IsGuessed = true;
            js.InvokeVoidAsync("Web.saveToStorage", "translations", JsonConvert.SerializeObject(Translations));
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

        public TranslationMultipleChoices CreateCardMultipleChoices(TranslationItem word, int wrongAnswerCount)
        {
            var wrongAnswers =
                Translations
                .Where(w => w.Spanish != word.Spanish)
                .OrderBy(ob => _random.Next())
                .Select(s => s.English)
                .Take(wrongAnswerCount)
                .ToList();

            wrongAnswers.Add(word.English);
            var allAnswers = wrongAnswers
                .OrderBy(ob => _random.Next())
                .ToList();

            return new TranslationMultipleChoices { Choices = allAnswers, Answer = word.English, Translation = word.Spanish };
        }

        public void ReSetGuesses()
        {
            Translations.ForEach(fe => { fe.IsGuessed = false; });
            js.InvokeVoidAsync("Web.saveToStorage", "translations", JsonConvert.SerializeObject(Translations));
            NotifyReStart();
        }

        public void Shuffle()
        {
            NotifyReStart();
        }

        public async void Reset()
        {
            await js.InvokeVoidAsync("Web.clearStorageItem", "translations");
            await js.InvokeVoidAsync("Web.clearStorageItem", "sheet-id");
            Translations = new List<TranslationItem>();
            SheetId = null;
            NotifyStateChanged();
            NotifySheetIdChanged();
        }
    }
}
