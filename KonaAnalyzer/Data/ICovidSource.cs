using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReactiveUI;

namespace KonaAnalyzer.Data
{
    public interface ICovidSource 
    {
        List<DayChange> Changes { get; }
        List<string> States { get; }
        List<string> Counties(string state);
        int Total(string state, string county, DateTime? date);
        int Deaths(string state, string county, DateTime? date);
        double ChangeRateByCounty(string state, string county);
        double ChangeRateByState(string state);
        Task LoadAsync();
        DateTime LastDate(string state);
        bool Loaded { get; }
    }
}
