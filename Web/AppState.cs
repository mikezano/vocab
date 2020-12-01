﻿using Microsoft.AspNetCore.Components;
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
        public string SelectedColour { get; private set; }
        private Random _random = new Random();

        [Inject]
        private IJSRuntime js { get; set; }


        public event Action OnChange;

        public void SetJsInterop(IJSRuntime js)
        {
            this.js = js;
        }

        public void SetColour(string colour)
        {
            SelectedColour = colour;
            NotifyStateChanged();
        }

        public void UpdateCorrectGuess(int index)
        {
            Translations[index].IsGuessed = true;
            if(js != null)
            {
                Console.WriteLine("JS: " + js);
            }
            js.InvokeVoidAsync("Web.saveToStorage", JsonConvert.SerializeObject(Translations));
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        public int GuessedCorrectlyCount => Translations.Count(c => c.IsGuessed);


        public void SetWords(List<TranslationItem> words)
        {
            Translations = words;
        }

        public List<TranslationMultipleChoices> GetMultipleChoiceSets(int count)
        {
            Random random = new Random();
            var cardDataSet = Translations
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
            NotifyStateChanged();
        }

    }
}
