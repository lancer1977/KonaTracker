﻿using System;
using System.Collections.Generic;
using System.Text;

namespace KonaAnalyzer.Models
{
    public class HomeMenuItem
    { 

        public string Title { get; set; }
        public override string ToString() => Title;
    }
}