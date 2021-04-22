using System;
using KonaAnalyzer.Data.Interface;

namespace KonaAnalyzer.Data.Model
{
    //Raw on the server

    //Calculated
    public class CountyChangeModel : IChange
    {
        public DateTime Date { get; set; }
        public string County { get; set; }
        public int Fips { get; set; }
        public string State { get; set; }
        public int Cases { get; set; }
        public int Deaths { get; set; }
        //public bool IsEstimate { get; set; }
    }
    //Over the wire csv
}
