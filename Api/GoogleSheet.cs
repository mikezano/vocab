using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vocab.Models;
namespace Vocab.Api
{
    public class GoogleSheetCell
    {
        public required string CellName { get; set; }
        public required string CellValue { get; set; }
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
            return translations.Skip(1).ToList();
        }

        public List<TranslationItem> FormatWords(object json)
        {
            ArgumentNullException.ThrowIfNull(json, nameof(json));
            string jsonString = json.ToString() ?? 
                throw new ArgumentException("json.ToString() returned null", nameof(json)); 
            JObject jsonObject = JObject.Parse(jsonString);

            var rows = jsonObject.SelectToken("table.rows") ??
                throw new ArgumentException("JSON does not contain 'table.rows' token", nameof(json));

            var translations = new List<TranslationItem>();
            translations = rows.Select(s =>
            {
                var word = s.SelectToken("c[0].v")?.ToString() ??
                    throw new ArgumentException("Failed to find word in JSON");
                var translation = s.SelectToken("c[1].v")?.ToString() ??
                    throw new ArgumentException("Failed to find translation in JSON");

                return new TranslationItem()
                {
                    Word = word,
                    Translation = translation,
                };

            }).ToList();
            return translations;
        }
    }
}
