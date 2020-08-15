using System.Runtime.CompilerServices;
using KonaAnalyzer.Data;
using KonaAnalyzer.Interfaces;
using KonaAnalyzer.Setup;
using Xamarin.Forms;
using ReactiveUI.Fody.Helpers;

namespace KonaAnalyzer.ViewModels
{
    public abstract class BaseViewModel : ReactiveUI.ReactiveObject
    {
        public BaseViewModel()
        {
            DataStore = IOC.Get<ICovidSource>();
            LocationStore = IOC.Get<ILocationSource>();
            PopulationDataStore = IOC.Get<IPopulationSource>();
        }

        public ICovidSource DataStore { get; }
        public ILocationSource LocationStore { get; }
        public IPopulationSource PopulationDataStore { get; }
        [Reactive] public bool IsBusy { get; set; }
        [Reactive] public string Title { get; set; }


    }
}
