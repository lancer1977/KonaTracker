using System.Collections.Generic;
using System.Threading.Tasks;
using KonaAnalyzer.Models;

namespace KonaAnalyzer.Interfaces
{
    public interface ILocationSource : IDataSource
    {
        IEnumerable<Location> Locations { get; }
        int GetId(string state, string county);

        Location GetLocation(int id);
        IEnumerable<string> Counties(string state);
        IEnumerable<string> States();
        Location GetLocation(string state, string county);
        Task LoadAsync();
        int GetFips(string state, string county);
    }
}