﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class TranslationItem
    {
        public string Spanish { get; set; } = "----";
        public string English { get; set; }
        public bool IsGuessed { get; set; }
    }
}
