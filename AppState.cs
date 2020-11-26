using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocab.Models;

namespace Vocab
{
    public class AppState
    {
        public List<BasicWord> Words { get; private set; }
        public string SelectedColour { get; private set; }
        private Random _random = new Random();
        

        public event Action OnChange;

        public void SetColour(string colour)
        {
            SelectedColour = colour;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();


        public void SetWords(List<BasicWord> words)
        {
            Words = words;
        }

        public List<CardDataMultipleChoices> GetCardDataSet()
        {
            Random random = new Random();
            var cardDataSet = Words
                .OrderBy(x => random.Next())
                .Take(9)
                .ToList()
                .Select(s => { return CreateCardMultipleChoices(s, 2); })
                .ToList();
            Console.WriteLine("GetCardDataSet");
            return cardDataSet;
        }

        public CardDataMultipleChoices CreateCardMultipleChoices(BasicWord word, int wrongAnswerCount)
        {
            var wrongAnswers =
                Words
                .Where(w => w.Spanish != word.Spanish)
                .OrderBy(ob => _random.Next())
                .Select(s => s.English)
                .Take(wrongAnswerCount)
                .ToList();

            wrongAnswers.Add(word.English);
            var allAnswers = wrongAnswers
                .OrderBy(ob => _random.Next())
                .ToList();

            return new CardDataMultipleChoices { Choices = allAnswers, Answer = word.English, Translation = word.Spanish };
        }



    }
}
