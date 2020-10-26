using System;
using KonaAnalyzer.Data;

namespace KonaAnalyzer.Models
{
    public class LiteDayChange : IChange
    {
        public DateTime date { get; set; }
        public Location Location { get; set; }
        public int cases { get; set; }
        public int deaths { get; set; }
        public string state => Location.State;
        public string county => Location.County;
        public int? fips => Location.Fips;
    }
}
