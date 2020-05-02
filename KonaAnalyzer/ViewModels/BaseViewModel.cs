using System.Runtime.CompilerServices;
using KonaAnalyzer.Data;
using Xamarin.Forms;
using ReactiveUI.Fody.Helpers;

namespace KonaAnalyzer.ViewModels
{
    public class BaseViewModel : ReactiveUI.ReactiveObject
    {
        public ICovidSource DataStore => DependencyService.Get<ICovidSource>();
        public IPopulationSource PopulationDataStore => DependencyService.Get<IPopulationSource>();
        [Reactive] public bool IsBusy { get; set; }
        [Reactive] public string Title { get; set; }


    }
}
