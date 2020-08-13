using System.Threading.Tasks;

namespace KonaAnalyzer.Interfaces
{
    public interface IDataSource
    {
        LoadedState LoadState { get; } 
        Task LoadAsync();
        Task Reload();
    }
}