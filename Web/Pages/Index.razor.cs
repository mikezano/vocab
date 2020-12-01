using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Web.Api;
using Web.Models;


namespace Web.Pages
{
    public partial class Index
    {
        [Inject]
        public GoogleSheet GoogleSheet { get; set; }
        [Inject]
        public AppState AppState { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; }

        public List<TranslationMultipleChoices> MultipleChoiceSets { get; set; } = new List<TranslationMultipleChoices>();
        private List<TranslationItem> _translations = new List<TranslationItem>();
        private int _visibleCardCount = 0;

        static readonly string SpreadsheetId = "1drjaaHkOex3jIoP4mhTNoSsCVi9OSouS9NFI601yIKY";

        protected override void OnInitialized()
        {
            AppState.SetJsInterop(JS);
        }
        protected override async Task OnInitializedAsync()
        {
            var storageTranslations = await JS.InvokeAsync<List<TranslationItem>>("Web.getStorageItem", "translations");

            if (storageTranslations == null)
            {
                _translations = await GoogleSheet.GetEntries(SpreadsheetId);
                await JS.InvokeVoidAsync("Web.saveToStorage", JsonConvert.SerializeObject(_translations));
            }
            else
            {
                _translations = storageTranslations;
            }

            AppState.SetWords(_translations.ToList());

            var dimensions = await JS.InvokeAsync<BrowserDimensions>("Web.getDimensions");
            _visibleCardCount = dimensions.GetVisibleCardCount();

            MultipleChoiceSets = AppState.GetMultipleChoiceSets(_visibleCardCount);
        }

        private void ReStart()
        {
            AppState.ReSetGuesses();
            MultipleChoiceSets = AppState.GetMultipleChoiceSets(_visibleCardCount);
        }
    }
}
