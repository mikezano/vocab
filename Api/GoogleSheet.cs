
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

        //google sheets v4 api
        private static string BASE_URL = "https://docs.google.com/spreadsheets/d/{0}/gviz/tq";
        private readonly HttpClient client;

        public GoogleSheet(HttpClient client)
        {
            this.client = client;
        }

        public async Task<List<TranslationItem>> GetEntries(string sheetId)
        {
            string url = String.Format(BASE_URL, sheetId);
            var entries = await client.GetStringAsync(url);
            entries = entries.Substring(47);
            entries = entries.Remove(entries.Length - 2);
            var translations = FormatWords(entries);
            return translations;
        }

        public List<TranslationItem> FormatWords(object json)
        {
            JObject o = JObject.Parse(json.ToString());

            var translations = new List<TranslationItem>();
            translations = o.SelectToken("table.rows").Select(s => new TranslationItem
            {
                Spanish = (string)s.SelectToken("c[0].v"),
                English = (string)s.SelectToken("c[1].v")
            }).ToList();

            return translations;
        }
    }
}
