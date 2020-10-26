using System;
using System.Collections.Generic;
using System.ComponentModel;
using KonaAnalyzer.Data;
using KonaAnalyzer.Models;

namespace KonaAnalyzer.Interfaces
{
    public interface ICovidSource : INotifyPropertyChanged, IDataSource
    { 
        int Total(string state, string county, DateTime? date);
        int Deaths(string state, string county, DateTime? date);
        //DateTime LastDate(string state);
        IEnumerable<IChange> MatchingBetween(string state, string countyName, DateTime startDay, DateTime endDay);

        int Total(int fips, DateTime? date);
        int Deaths(int fips, DateTime? date);
        //DateTime LastDate(string state);
        IEnumerable<IChange> MatchingBetween(int fips, DateTime startDay, DateTime endDay);


        //DateTime EarliestDate(string first);
        DateTime Latest { get; }
        DateTime Earliest { get; }
        IEnumerable<IChange> GenerateEstimates(int days);
    }

}
