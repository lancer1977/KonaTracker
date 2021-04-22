using System.Runtime.Serialization;

namespace KonaAnalyzer.Data.Model
{ 
 
    public class RawPopulation
    { 
        public string state { get; set; }
        public string county { get; set; }
        public int population { get; set; }
    }
}