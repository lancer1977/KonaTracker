using System;

namespace KonaAnalyzer.Data.Model
{
    public readonly struct ChartUpdate
    {
        public string State { get; }
        public int Fips { get; }
        public string County { get; }
        public DateTime StartDay { get; }
        public DateTime EndDay { get; }

        public ChartUpdate(string state, string county,DateTime startDay, DateTime endDay, int fips)
        {
            Fips = fips;
            State = state;
            County = county ?? "All";
            StartDay = startDay;
            EndDay = endDay;
        }
    }
}