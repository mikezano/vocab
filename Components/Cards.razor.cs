using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Vocab.Models;

namespace Vocab.Components
{
    partial class Cards
    {
        [Parameter]
        public EventCallback OnAllCardsGuessed { get; set; }
        private List<TranslationMultipleChoices> Choices { get; set; } = new();
        public int VisibleCardCount { get; set; } = 0;

        private int _selectedCardIndex = -1;

        protected override async Task OnInitializedAsync()
        {

            var dimensions = await JS.InvokeAsync<BrowserDimensions>("Web.getDimensions");
            VisibleCardCount = dimensions.GetVisibleCardCount();
            AppState.OnTranslationsLoaded += CreateMultipleChoices;
            AppState.OnReStart += CreateMultipleChoices;
            if (AppState.Translations.Any())
            {
                CreateMultipleChoices();
            }
        }

        public void Dispose()
        {
            AppState.OnTranslationsLoaded -= CreateMultipleChoices;
            AppState.OnReStart -= CreateMultipleChoices;
        }

        public void CreateMultipleChoices()
        {
            Choices = AppState.GetMultipleChoiceSets(VisibleCardCount);
            StateHasChanged();
        }

        private async Task HandleGuess((Answer answer, int cardId) args)
        {
            if (args.answer.IsCorrect)
            {
                int index = AppState.Translations.FindIndex(i => i.Word == args.answer.Word);
                await AppState.UpdateCorrectGuess(index);
            }
            else
            {
                await AppState.UpdateIncorrectGuesses();
    
            }

            if (Choices.Count == 1 && !args.answer.IsCorrect)
            {
                return;
            }

            _selectedCardIndex = args.cardId;
            //Actually shouldn't need to get another translation if only 1 left


            //When one card is left, this gets here and returns a null
            //It should give me back the same if 1 is left and it is wrong
            //Should truly be null if the last one is correct and nothing left
            //The query for guessed should still return 1
           
            var displayedWords = Choices
                .Where(i => i.Word != args.answer.Word)
                .Select(i => i.Word)
                .ToList();
            var nextTranslation = AppState.GetNextMultipleChoiceSet(VisibleCardCount, displayedWords);

            if (nextTranslation != null)
            {
                Choices[_selectedCardIndex] = nextTranslation;
            }
            else
            {
                Choices.RemoveAt(_selectedCardIndex);
            }

            if (Choices.Count == 0)
            {
                await OnAllCardsGuessed.InvokeAsync();
            }

        }

        public void HandleFlipDone()
        {
            StateHasChanged();
        }
    }
}
