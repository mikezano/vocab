using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocab.Models;

namespace Vocab
{
    public class AppState
    {
        public string SelectedColour { get; private set; }
        

        public event Action OnChange;

        public void SetColour(string colour)
        {
            SelectedColour = colour;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        public List<BasicWord> Words { get; private set; }
        public void SetWords(List<BasicWord> words)
        {
            Words = words;
        }

        public List<BasicWord> GetWordSet()
        {
            Random random = new Random();
            return Words.OrderBy(x => random.Next()).Take(3).ToList();
        }


    }
}
