using System;
using KonaAnalyzer.Data.Interface;

namespace KonaAnalyzer.Data.Model
{
    public class LiteDayChange : IChange
    {
        public DateTime Date { get; set; }
        public LocationModel Location { get; set; }
        public int Cases { get; set; }
        public int Deaths { get; set; }
        public string State => Location.State;
        public string County => Location.County;
        public int Fips => Location.Fips ?? -1;
    }
}
