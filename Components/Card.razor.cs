using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Vocab.Models;

namespace Vocab.Components
{
    public partial class Card
    {
        [Parameter]
        public required TranslationMultipleChoices MultipleChoices { get; set; }
        [Parameter]
        public int Id { get; set; }
        [Parameter]
        public EventCallback<Answer> OnCorrect { get; set; }

        public string CardAnimationClass { get; set; } = String.Empty;
        public string CardHalfFlipStart { get; set; } = String.Empty;
        public string CardHalfFlipEnd { get; set; } = String.Empty;

        private ElementReference ReferenceToInputControl;

        private ElementReference _cardRef;
        private DotNetObjectReference<Card>? _dotNetRef;

        public bool? IsCorrect { get; set; }
        public string CurrentGuess { get; set; } = String.Empty;


        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _dotNetRef = DotNetObjectReference.Create(this);
            }
            return Task.CompletedTask; // Ensure all code paths return a Task
        }

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
                Word = MultipleChoices.Answer,
                Translation = MultipleChoices.Word,
                ReplacementIndex = Id,
                IsCorrect = IsCorrect.HasValue && IsCorrect.Value == true
            });

            await JS.InvokeVoidAsync("Web.clearRadioButtons");
            CardHalfFlipEnd = "card-half-flip-end";
        }

        public async Task OnHalfFlipEndComplete(string animationName)
        {
            IsCorrect = null;
            CardAnimationClass = SetCardAnimationClass();
            CardHalfFlipStart = String.Empty;
            CardHalfFlipEnd = String.Empty;
        }

        private async void Guess(ChangeEventArgs args)
        {
            Console.WriteLine($"Guess: {args.Value}");
            var answer = args.Value.ToString();
            //JSRuntime.InvokeVoidAsync("Vocab.setFocus", ReferenceToInputControl);
            IsCorrect = answer == MultipleChoices.Answer;
            CardAnimationClass = SetCardAnimationClass();

            //await needed
            await JS.InvokeVoidAsync("animationInterop.onAnimationEnd", _cardRef, _dotNetRef, nameof(OnAnimationRevealEnd));
        }

        [JSInvokable]
        public void OnAnimationRevealEnd()
        {
            //StatusMessage = "Animation has finished!";
            Console.WriteLine("Animation has finished!");
            StateHasChanged(); // Refresh UI
        }

        private string SetCardAnimationClass()
        {
            Console.WriteLine($"Setting card animation class for IsCorrect: {IsCorrect}");
            string result = String.Empty;
            if (IsCorrect.HasValue)
            {
                result = IsCorrect.Value ? "card-answer-reveal" : "card-answer-wrong";
            }
            return result;
        }
    }
}
