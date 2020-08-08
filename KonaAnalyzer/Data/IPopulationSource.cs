using System.Threading.Tasks;

namespace KonaAnalyzer.Data
{
    public interface IPopulationSource
    {
        int Population(string state, string county);
        Task LoadAsync();
    }
}