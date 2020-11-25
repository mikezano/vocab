using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocab.Models
{
    public class Answer
    {
        public BasicWord Word { get; set; }
        public int ReplacementIndex { get; set; }
        public bool IsCorrect { get; set; }
    }
}
