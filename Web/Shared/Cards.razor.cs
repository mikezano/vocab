using System;
using Web.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace Web.Shared
{
    public partial class Cards
    {
        [Parameter]
        public List<TranslationMultipleChoices> MultipleChoiceSets { get; set; } = new List<TranslationMultipleChoices>();

        private static Random random = new Random();

        public void OnCorrect(Answer answer)
        {
            int indexOfWord = AppState.Translations.FindIndex(f => f.Spanish == answer.Translation);
            if (answer.IsCorrect)
            {
                AppState.UpdateCorrectGuess(indexOfWord);
            }

            //Next word has to be not guessed and not on screen
            //var notGuessed = AppState.Words.Where(w => !w.IsGuessed && Data.FindIndex(f => f.Translation == w.Spanish) == -1).ToList();
            var notGuessed = AppState.Translations.Where(w => !w.IsGuessed).ToList();
            Console.WriteLine("Not Guessed " + notGuessed.Count.ToString());
            var notOnScreen = notGuessed.Where(w => MultipleChoiceSets.FindIndex(fi => fi.Translation == w.Spanish) == -1).ToList();
            Console.WriteLine("Not On Screen " + notOnScreen.Count.ToString());
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
