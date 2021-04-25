using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KonaAnalyzer.Data.Interface
{
    public interface ICovidSourceAsync : ICovidSource
    {
        Task<int> TotalAsync(string state, DateTime? date);
        Task<int> TotalAsync(int fips, DateTime? date);
        Task<int> TotalAsync(DateTime date);
        Task<int> DeathsAsync(string state, DateTime? date);
        Task<int> DeathsAsync(int fips, DateTime? date);
        Task<int> DeathsAsync(DateTime date);
        Task<IEnumerable<IChange>> MatchingBetweenAsync(int fips, DateTime startDay, DateTime endDay);
        Task<IEnumerable<IChange>> GenerateEstimatesAsync(int days);
    }

}
