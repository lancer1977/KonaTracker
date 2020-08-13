using System;
using System.Collections.Generic;
using System.ComponentModel;
using KonaAnalyzer.Data;

namespace KonaAnalyzer.Interfaces
{
    public interface ICovidSource : INotifyPropertyChanged, IDataSource
    {   
        int Total(string state, string county, DateTime? date);
        int Deaths(string state, string county, DateTime? date);
        //DateTime LastDate(string state);
        IEnumerable<IChange> CountyChanges(string state, string countyName, DateTime startDay, DateTime endDay);

        //DateTime EarliestDate(string first);
        DateTime Latest { get; }
        DateTime Earliest { get; }
    }

}
