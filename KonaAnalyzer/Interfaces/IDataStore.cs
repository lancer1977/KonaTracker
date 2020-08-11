using System.Collections.Generic;
using System.Threading.Tasks;

namespace KonaAnalyzer.Interfaces
{
    public interface IDataStore<T>
    {
        Task<bool> AddItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(string id);
        Task<T> GetItemAsync(string id);
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
    }

    public interface IDataSource
    {
        LoadedState LoadState { get; }
        Task LoadAsync();
        Task Reload();
    }

    public enum LoadedState
    {
        Unloaded,
        Loading,
        Loaded
    }
}
