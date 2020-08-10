using System;

namespace KonaAnalyzer.Data
{
    public class LiteDayChange : IChange
    {
        public DateTime date { get; set; }
        public Location Location { get; set; }
        public int cases { get; set; }
        public int deaths { get; set; }
    }
}
