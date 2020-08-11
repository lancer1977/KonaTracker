using System;
using System.Collections.Generic;
using System.ComponentModel; 
using KonaAnalyzer.Services; 

namespace KonaAnalyzer.Data
{
    public interface ICovidSource : INotifyPropertyChanged, IDataSource
    {
        IEnumerable<IChange> Changes { get; }
        List<string> States { get; }
        List<string> Counties(string state);
        int Total(string state, string county, DateTime? date);
        int Deaths(string state, string county, DateTime? date);
        double ChangeRateByCounty(string state, string county);
        double ChangeRateByState(string state);
        //DateTime LastDate(string state);
        IEnumerable<IChange> CountyChanges(string state, string countyName, DateTime startDay, DateTime endDay);

        //DateTime EarliestDate(string first);
        DateTime Latest { get; }
        DateTime Earliest { get; }
    }

}
