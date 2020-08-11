using System.Runtime.CompilerServices;
using KonaAnalyzer.Data;
using KonaAnalyzer.Interfaces;
using KonaAnalyzer.Setup;
using Xamarin.Forms;
using ReactiveUI.Fody.Helpers;

namespace KonaAnalyzer.ViewModels
{
    public class BaseViewModel : ReactiveUI.ReactiveObject
    {
        public ICovidSource DataStore => IOC.Get<ICovidSource>();
        public IPopulationSource PopulationDataStore => IOC.Get<IPopulationSource>();// InMemoryPopulationSource.Instance;
        [Reactive] public bool IsBusy { get; set; }
        [Reactive] public string Title { get; set; }


    }
}
