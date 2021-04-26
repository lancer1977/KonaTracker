using KonaAnalyzer.Data.Interface;
using ReactiveUI;

namespace KonaAnalyzer.Blazor.ViewModels
{
    public abstract class BaseViewModel : ReactiveObject
    {
        protected BaseViewModel(ICovidSourceAsync datastore, ILocationSource locationStore, IMaskSource maskSource)
        {
            DataStore = datastore;
            LocationStore = locationStore;
            MaskStore = maskSource;
        }

        public IMaskSource MaskStore { get; }
        public ICovidSourceAsync DataStore { get; }
        public ILocationSource LocationStore { get; }
        public bool IsBusy { get; set; }
        public string Title { get; set; }

    }
}