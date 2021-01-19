using System;
using System.Runtime.Serialization;
using KonaAnalyzer.Data;

namespace KonaAnalyzer.Models
{
    public class DayChange : IChange
    {
        public DateTime date { get; set; }

        public string county { get; set; }
        public int? fips { get; set; }
        public string state { get; set; }
        public int cases { get; set; }
        public int? deaths { get; set; }
        //public bool IsEstimate { get; set; }
    }
}
