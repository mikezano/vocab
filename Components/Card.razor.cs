using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;
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
        public EventCallback<(Answer Answer, int Id)> OnSelect { get; set; }
        [Parameter]
        public EventCallback OnFlipDone { get; set; }


        public Dictionary<string, bool> AnimationClasses = new Dictionary<string, bool>
        {
            { "card-reveal", false },
            { "card-incorrect", false },
            { "card-correct", false },
            { "card-conceal", false },
        };

        public string ActiveAnimationClasses => 
            string.Join(
                " ", 
                AnimationClasses.Where(kvp => kvp.Value).Select(kvp => kvp.Key));

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

        private async Task SetupNextAnimation(string nextAnimationMethod)
        {
            await JS.InvokeVoidAsync(
                "animationInterop.onAnimationEnd",
                _cardRef,
                _dotNetRef,
                nextAnimationMethod
            );
        }

        private async void Guess(ChangeEventArgs args)
        {
            var answer = args.Value.ToString();
            IsCorrect = answer == MultipleChoices.Answer;

            await SetupNextAnimation(nameof(OnAnimationRevealEnd));
            AnimationClasses["card-reveal"] = true;
            StateHasChanged(); // Refresh UI so that classes are applied and animation triggered           
        }

        [JSInvokable]
        public async void OnAnimationRevealEnd()
        {
            if(!IsCorrect.HasValue)
            {
                return;
            }

            await SetupNextAnimation(nameof(OnAnimationCorrectnessEnd));
            AnimationClasses[IsCorrect.Value ? "card-correct": "card-incorrect"] = true;
            StateHasChanged(); // Refresh UI
        }


        [JSInvokable]
        public async void OnAnimationCorrectnessEnd()
        {

            await OnSelect.InvokeAsync(
                (
                    new Answer
                    {
                        Word = MultipleChoices.Word,
                        Translation = MultipleChoices.Answer,
                        IsCorrect = IsCorrect.HasValue && IsCorrect.Value
                    }, 
                    Id
                )
            );
            await SetupNextAnimation(nameof(OnAnimationConcealEnd));
            await JS.InvokeVoidAsync("Web.clearRadioButtons");
            
            AnimationClasses["card-conceal"] = true;
            StateHasChanged(); // Refresh UI

        }

        [JSInvokable]
        public  async Task OnAnimationConcealEnd()
        {
            AnimationClasses.Keys.ToList().ForEach(key => AnimationClasses[key] = false);
            await OnFlipDone.InvokeAsync();
            StateHasChanged(); // Refresh UI   
        }
    }
}
