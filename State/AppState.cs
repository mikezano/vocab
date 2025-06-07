using Microsoft.JSInterop;
using Newtonsoft.Json;
using Vocab.Models;

namespace Vocab.State
{
    public class AppState
    {
        public List<TranslationItem> Translations { get; private set; } = new List<TranslationItem>();
        public string? SheetId { get; private set; }
        public int IncorrectGuesses { get; set; } = 0;
        private Random _random = new Random();

        private IJSRuntime js;

        public event Action? OnChange;
        public event Action? OnSetSheetId;
        public event Action? OnReStart;

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
            js.InvokeVoidAsync("Web.saveToStorage", "translations", JsonConvert.SerializeObject(Translations));
            NotifyStateChanged();
        }
        public void UpdateIncorrectGuesses()
        {
            IncorrectGuesses++;
            js.InvokeVoidAsync("Web.saveToStorage", "incorrectGuesses", IncorrectGuesses);
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

            wrongAnswers.Add(item.Word);
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
            Translations.ForEach(fe => { fe.IsGuessed = false; });
            js.InvokeVoidAsync("Web.saveToStorage", "translations", JsonConvert.SerializeObject(Translations));
            NotifyReStart();
            IncorrectGuesses = 0;
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
