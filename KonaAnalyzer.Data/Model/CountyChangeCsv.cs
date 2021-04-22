using System;

namespace KonaAnalyzer.Data.Model
{
    public class CountyChangeCsv
    {
        public DateTime date { get; set; }

        public string county { get; set; }
        public int? fips { get; set; }
        public string state { get; set; }
        public int? cases { get; set; }
        public int? deaths { get; set; }

        //public bool IsEstimate { get; set; }
    }
}