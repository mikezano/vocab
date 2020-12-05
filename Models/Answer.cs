using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class Answer
    {
        public string Value { get; set; }
        public string Translation { get; set; }
        public int ReplacementIndex { get; set; }
        public bool IsCorrect { get; set; }
    }
}
