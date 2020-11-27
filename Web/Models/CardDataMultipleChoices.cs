using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class CardDataMultipleChoices
    {
        public string Answer { get; set; }
        public string Translation { get; set; }
        public List<string> Choices { get; set; }
    }
}
