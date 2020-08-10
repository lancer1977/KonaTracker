using System.Threading.Tasks;
using KonaAnalyzer.Services;

namespace KonaAnalyzer.Data
{
    public interface IPopulationSource : IDataSource
    {
        int Population(string state, string county);
       
    }
}