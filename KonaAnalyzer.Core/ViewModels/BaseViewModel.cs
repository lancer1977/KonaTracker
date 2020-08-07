using KonaAnalyzer.Data;
using ReactiveUI.Fody.Helpers;

namespace KonaAnalyzer.Core.ViewModels
{
    public class BaseViewModel : ReactiveUI.ReactiveObject
    { 

        public BaseViewModel(ICovidSource covid, IPopulationSource populationSource)
        {
            DataStore = covid;
            PopulationDataStore = populationSource;
        }
        public ICovidSource DataStore { get; }
        public IPopulationSource PopulationDataStore { get; }
        [Reactive] public bool IsBusy { get; set; }
        [Reactive] public string Title { get; set; }


    }
}
