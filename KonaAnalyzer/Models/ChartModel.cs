﻿using System;

namespace KonaAnalyzer.Models
{
    public class ChartModel
    {
        public DateTime Date { get; set; }

        public double Change { get; set; }

        //[PrimaryKey,AutoIncrement]
        //public int Id { get; set; }
    }
}