using System;

namespace KonaAnalyzer.Data
{
    public interface IChange
    {
        DateTime Date { get; }
        int Cases { get; }
        int Deaths { get; }
    }
}
