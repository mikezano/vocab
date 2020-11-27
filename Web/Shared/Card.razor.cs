using System;
using Web.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Web.Shared
{
    public partial class Card
    {
        [Parameter]
        public CardDataMultipleChoices MultipleChoices { get; set; }
        [Parameter]
        public int Id { get; set; }
        [Parameter]
        public EventCallback<Answer> OnCorrect { get; set; }
        [Parameter]
        public string[] Choices { get; set; }

        public string CardAnimationClass { get; set; } = String.Empty;
        public string CardHalfFlipStart { get; set; } = String.Empty;
        public string CardHalfFlipEnd { get; set; } = String.Empty;

        private ElementReference ReferenceToInputControl;

        public bool? IsCorrect { get; set; }
        public string CurrentGuess { get; set; }

        public async Task OnRevealComplete(string animationName)
        {
            if (IsCorrect.Value)
            {
                CurrentGuess = "";
                CardHalfFlipStart = "card-half-flip-start";
            }
        }

        public async Task OnWrongAnswerComplete()
        {
            IsCorrect = null;
            CardAnimationClass = SetCardAnimationClass();
            CardHalfFlipStart = "card-half-flip-start";
        }

        public async Task OnHalfFlipStartComplete(string cssAnimationName)
        {

            await OnCorrect.InvokeAsync(new Answer
            {
                Value = MultipleChoices.Answer,
                Translation = MultipleChoices.Translation,
                ReplacementIndex = Id,
                IsCorrect = IsCorrect.HasValue && IsCorrect.Value == true
            });

            await JSRuntime.InvokeVoidAsync("Web.clearRadioButtons");
            CardHalfFlipEnd = "card-half-flip-end";
        }

        public async Task OnHalfFlipEndComplete(string animationName)
        {
            IsCorrect = null;
            CardAnimationClass = SetCardAnimationClass();
            CardHalfFlipStart = String.Empty;
            CardHalfFlipEnd = String.Empty;
        }

        private void Guess(ChangeEventArgs args)
        {
            var answer = args.Value.ToString();
            //JSRuntime.InvokeVoidAsync("Vocab.setFocus", ReferenceToInputControl);
            IsCorrect = answer == MultipleChoices.Answer;
            CardAnimationClass = SetCardAnimationClass();
        }


        private string SetCardAnimationClass()
        {
            string result = String.Empty;
            if (IsCorrect.HasValue)
            {
                result = IsCorrect.Value ? "card-answer-reveal" : "card-answer-wrong";
            }
            return result;
        }
    }
}
