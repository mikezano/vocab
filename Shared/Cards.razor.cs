using System;
using Vocab.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace Vocab.Shared
{
    public partial class Cards
    {
        [Parameter]
        public List<CardDataMultipleChoices> Data { get; set; } = new List<CardDataMultipleChoices>();

        private static Random random = new Random();

        public void OnCorrect(Answer answer)
        {
            int indexOfWord = AppState.Words.FindIndex(f => f.Spanish == answer.Translation);
            if (answer.IsCorrect)
            {
                AppState.UpdateCorrectGuess(indexOfWord);
            }

            //Next word has to be not guess  ed and not on screen
            //var notGuessed = AppState.Words.Where(w => !w.IsGuessed && Data.FindIndex(f => f.Translation == w.Spanish) == -1).ToList();
            var notGuessed = AppState.Words.Where(w => !w.IsGuessed).ToList();
            Console.WriteLine("Not Guessed " + notGuessed.Count.ToString());
            var notOnScreen = notGuessed.Where(w => Data.FindIndex(fi => fi.Translation == w.Spanish) == -1).ToList();
            Console.WriteLine("Not On Screen " + notOnScreen.Count.ToString());
            if (notOnScreen.Count == 0 && !answer.IsCorrect)
            {
                notOnScreen.Add(AppState.Words[indexOfWord]);
            }

            int nextIndex = random.Next(notOnScreen.Count);
            BasicWord nextWord = new BasicWord();
            if (notOnScreen.Count > 0)
            {
                nextWord = notOnScreen[nextIndex];
                Data[answer.ReplacementIndex] = AppState.CreateCardMultipleChoices(nextWord, 2);
            }
            else
            {
                Data[answer.ReplacementIndex] = new CardDataMultipleChoices { Answer = "----", Choices = null, Translation = "----"};
            }
        }
    }
}
