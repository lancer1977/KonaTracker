using System;
using System.Collections.Generic;
using System.Linq;

namespace KonaAnalyzer.SqlData
{
    public static class EnumerableExtensions
    {
        public static (T, T) GetFirstAndLastT<T2, T>(this IEnumerable<T2> source, Func<T2, T> getT)
        {
            var items = source.ToList();
            var first = items.Select(getT).FirstOrDefault();
            var last = items.Select(getT).LastOrDefault();
            return (first, last);
        }
    }
}