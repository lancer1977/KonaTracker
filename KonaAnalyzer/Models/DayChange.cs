using System;
using System.Runtime.Serialization;

namespace KonaAnalyzer.Data
{
    [DataContract]
    public class DayChange: IChange
    { 
        public DateTime date { get; set; }

        public DateTime Date => date;

        public string county { get; set; } 
        public string state { get; set; } 
        public int cases { get; set; }

        public int Cases => cases;

        public int deaths { get; set; }

        public int Deaths => deaths;
    }
}
