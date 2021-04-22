using System;

namespace KonaAnalyzer.Data.Interface
{
    public interface IChange
    {
        DateTime Date { get; }
        int Cases { get; }
        int Deaths { get; }
        string State { get; }
        string County { get;  }
        int Fips { get; }
    }
}
