using System.Collections.Generic;
using System.Threading.Tasks;
using KonaAnalyzer.Data.Model;

namespace KonaAnalyzer.Data.Interface
{
    public interface ILocationSource : IDataSource
    {
        IEnumerable<LocationModel> Locations { get; }
        int GetId(string state, string county);

        LocationModel GetLocation(int id);
        IEnumerable<string> Counties(string state);
        IEnumerable<string> States();
        LocationModel GetLocation(string state, string county);
        Task LoadAsync();
        int GetFips(string state, string county);
        Dictionary<string, int> GetStateFipsDictionary();
    }
}