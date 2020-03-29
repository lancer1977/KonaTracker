using System;
using System.Runtime.Serialization;

namespace KonaAnalyzer.Data
{
    [DataContract]
    public class DayChange
    { 
        public DateTime date { get; set; } 
        public string county { get; set; } 
        public string state { get; set; } 
        public int cases { get; set; } 
        public int deaths { get; set; }


    }
}
