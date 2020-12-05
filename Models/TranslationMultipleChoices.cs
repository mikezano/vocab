using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class TranslationMultipleChoices
    {
        public string Answer { get; set; }
        public string Translation { get; set; }
        public List<string> Choices { get; set; }
    }
}
