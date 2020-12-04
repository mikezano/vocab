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
        public AppState AppState { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; }

        protected override async Task OnInitializedAsync()
        {
            AppState.SetJsInterop(JS);
            AppState.OnSetSheetId += StateHasChanged;

            var currentSheetId = await JS.InvokeAsync<string>("Web.getStorageItemAsString", "sheet-id");
            AppState.SetSheetId(currentSheetId);
        }

        public void Dispose()
        {
            AppState.OnSetSheetId -= StateHasChanged;
        }

        private void ReStart()
        {
            AppState.ReSetGuesses();
        }

        private void Shuffle()
        {
            AppState.Shuffle();
        }
    }
}
