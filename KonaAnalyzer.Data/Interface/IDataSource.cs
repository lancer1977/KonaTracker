using System.Threading.Tasks;

namespace KonaAnalyzer.Data.Interface
{
    public interface IDataSource
    {
        LoadedState LoadState { get;  } 
        Task LoadAsync();
        Task Reload();
    }
}