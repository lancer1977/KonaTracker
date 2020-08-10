using System.Collections.Generic;
using System.Threading.Tasks;

namespace KonaAnalyzer.Data
{
    public interface ILocationSource
    {
        List<Location> Locations { get; }
        int GetId(string state, string county);

        Location GetLocation(int id);
        IEnumerable<string> Counties(string state);
        IEnumerable<string> States();
        Location GetLocation(string state, string county);
        Task LoadAsync();
    }
}