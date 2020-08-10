using System;

namespace KonaAnalyzer.Data
{
    public class LiteDayChange : IChange
    {
        public DateTime Date { get; set; }
        public Location Location { get; set; }
        public int Cases { get; set; }
        public int Deaths { get; set; }
    }
}
