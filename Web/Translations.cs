using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.IO;

namespace Web
{
    public class Translations
    {
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static readonly string ApplicationName = "Vocab";
        static readonly string SpreadsheetId = "1drjaaHkOex3jIoP4mhTNoSsCVi9OSouS9NFI601yIKY";
        static readonly string sheet = "words";
        static SheetsService service;

        public Translations()
        {
            GoogleCredential credential;
            
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                .CreateScoped(Scopes);
            }

            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            //ReadEntries();
            //CreateEntry();
            //UpdateEntry();
           // DeleteEntry();
        }

        public Translations(string json)
        {
            GoogleCredential credential = 
                GoogleCredential
                .FromJson(json)
                .CreateScoped(Scopes);


            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            //ReadEntries();
            //CreateEntry();
            //UpdateEntry();
            // DeleteEntry();
        }


        public IList<IList<object>> ReadEntries()
        {
            var range = $"{sheet}!A:B";
            var request = service.Spreadsheets.Values.Get(SpreadsheetId, range);

            var response = request.Execute();
            var values = response.Values;
            return values;
            //if (values != null && values.Count > 0)
            //{
            //    foreach (var row in values)
            //    {
            //        Console.WriteLine("{0} {1}", row[0], row[1]);
            //    }
            //}
        }
    }
}
