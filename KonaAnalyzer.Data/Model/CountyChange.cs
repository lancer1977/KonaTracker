using System;

namespace KonaAnalyzer.Data.Model
{
    public class CountyChange
    {
        public DateTime Date { get; set; }
        public int Fips { get; set; }
        public int Cases { get; set; }
        public int Deaths { get; set; }
    }
}