using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class TranslationItem
    {
        public string From { get; set; } = "----";
        public string To { get; set; }
        public bool IsGuessed { get; set; }

        public TranslationItem Clone()
        {
            return new TranslationItem()
            {
                From = From,
                To = To,
                IsGuessed = IsGuessed   
            };
        }
    }
}
