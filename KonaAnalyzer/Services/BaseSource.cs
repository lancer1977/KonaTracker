using System.Threading.Tasks;
using KonaAnalyzer.Services;

using System.ComponentModel;
using DynamicData.Annotations;
using System.Runtime.CompilerServices;

namespace KonaAnalyzer.Data
{
    public abstract class BaseSource : IDataSource, INotifyPropertyChanged
    {
        private LoadedState _loadState;

        public async Task Reload()
        {
            LoadState = LoadedState.Unloaded;
            await LoadAsync();
        }

        public async Task LoadAsync()
        {
            if (LoadState != LoadedState.Unloaded) return;
            LoadState = LoadedState.Loading;
            await UpdateItems();
            LoadState = LoadedState.Loaded;

        }
        protected abstract Task UpdateItems();

        public LoadedState LoadState
        {
            get
            {
                return _loadState;
            }
            set
            {
                if (_loadState == value) return;
                _loadState = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}