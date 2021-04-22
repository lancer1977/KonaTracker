using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KonaAnalyzer.Data.Interface
{
    public interface ICovidSource : INotifyPropertyChanged, IDataSource
    {
        int Total(string state, DateTime? date);
        int Total(int fips, DateTime? date);

        int Deaths(string state, DateTime? date);
        int Deaths(int fips, DateTime? date); 

        IEnumerable<IChange> MatchingBetween(int fips, DateTime startDay, DateTime endDay);  
        DateTime Latest { get; }
        DateTime Earliest { get; } 
    }

}
