
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Web.Models;



namespace Web.Api
{
    public class GoogleSheetCell
    {
        public string CellName { get; set; }
        public string CellValue { get; set; }
    }

    public class GoogleSheet
    {
        private static string BASE_URL = "https://spreadsheets.google.com/feeds/cells/{0}/od6/public/basic?alt=json";
        private readonly HttpClient client;

        public GoogleSheet(HttpClient client)
        {
            this.client = client;
        }

        public async Task<List<TranslationItem>> GetEntries(string sheetId)
        {
            string url = String.Format(BASE_URL, sheetId);
            var entries = await client.GetFromJsonAsync<object>(url);
            var translations = FormatWords(entries);
            return translations;
        }

        public List<TranslationItem> FormatWords(object json)
        {
            JObject o = JObject.Parse(json.ToString());

            List<GoogleSheetCell> cells =
                o.SelectToken("feed.entry")
                .Select(s => new GoogleSheetCell
                {
                    CellName = (string)s.SelectToken("title.$t"),
                    CellValue = (string)s.SelectToken("content.$t")
                }).ToList();


            var columnA = cells.Where(w => w.CellName.StartsWith('A') && w.CellName != "A1").Select(s => s.CellValue).ToList();
            var columnB = cells.Where(w => w.CellName.StartsWith('B') && w.CellName != "B1").Select(s => s.CellValue).ToList();

            var translations = new List<TranslationItem>();
            for(int i=0; i < columnA.Count; i++)
            {
                translations.Add(new TranslationItem { Spanish = columnA[i], English = columnB[i] });
            }
            return translations;
        }
    }
}
