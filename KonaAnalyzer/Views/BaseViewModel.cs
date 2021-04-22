using System.Threading.Tasks;
using KonaAnalyzer.Data.Interface;
using KonaAnalyzer.Services;
using KonaAnalyzer.Setup;
using ReactiveUI.Fody.Helpers;

namespace KonaAnalyzer.ViewModels
{
    public abstract class BaseViewModel : ReactiveUI.ReactiveObject
    {
        protected BaseViewModel(ICovidSource datastore, ILocationSource locationStore, IMaskSource maskSource)
        {
            DataStore = datastore;
            LocationStore = locationStore; 
            MaskStore = maskSource;
        }

        public IMaskSource MaskStore { get; }
        public ICovidSource DataStore { get; }
        public ILocationSource LocationStore { get; } 
        [Reactive] public bool IsBusy { get; set; }
        [Reactive] public string Title { get; set; }

        public virtual async Task OnAppearing()
        {

        }
    }
}
