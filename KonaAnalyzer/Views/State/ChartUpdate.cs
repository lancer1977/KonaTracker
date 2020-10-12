using System;

namespace KonaAnalyzer.ViewModels
{
    public readonly struct ChartUpdate
    {
        public string State { get; }
        public string County { get; }
        public DateTime StartDay { get; }
        public DateTime EndDay { get; }

        public ChartUpdate(string state, string county, DateTime startDay, DateTime endDay)
        {
            State = state;
            County = county ?? "All";
            StartDay = startDay;
            EndDay = endDay;
        }
    }
}