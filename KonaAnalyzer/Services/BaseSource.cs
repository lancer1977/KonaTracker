using System.ComponentModel;
using System.Threading.Tasks;
using KonaAnalyzer.Data.Interface;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace KonaAnalyzer.Services
{
    public abstract class BaseSource : ReactiveObject, IDataSource, INotifyPropertyChanged
    {


        public async Task Reload()
        {
            LoadState = LoadedState.Unloaded; 
        }

   
        public async Task LoadAsync()
        {
            if (LoadState != LoadedState.Unloaded) return;
            LoadState = LoadedState.Loading;
            await UpdateItems();
            LoadState = LoadedState.Loaded;

        }
        protected abstract Task UpdateItems();
        [Reactive] public LoadedState LoadState { get; set; }


    }
}