using System;

namespace KonaAnalyzer.Data
{
    public interface IChange
    {
        DateTime date { get; }
        int cases { get; }
        int deaths { get; }
    }
}
