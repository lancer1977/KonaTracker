using System.Reactive;
using System.Threading.Tasks;
using KonaAnalyzer.Data;
using ReactiveUI;
using Xamarin.Forms;
using System;
using System.Diagnostics;
using KonaAnalyzer.Views;
using System.Windows.Input;
using KonaAnalyzer.Interfaces;

namespace KonaAnalyzer.ViewModels
{
    public class LoadingViewModel : BaseViewModel
    { 
        public LoadingViewModel(ICovidSource covidSource, IPopulationSource populationSource,ILocationSource locationSource)
        {
            Title = "Loading ...";

            LoadCommand = ReactiveCommand.CreateFromTask(async x=>await LoadAsync()); 
            LoadCommand.IsExecuting.Subscribe(x => {
                IsBusy = x;
                Debug.WriteLine($"Busy {IsBusy} sent:{x}");
            });
            LoadCommand.ThrownExceptions.Subscribe(OnException);
            _locationSource = locationSource;
            _populationSource = populationSource;
            _covidSource = covidSource;
        }

        private void OnException(Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }

        public   void OnAppearing()
        { 
            LoadCommand.Execute().Subscribe();
        }
        
        public ReactiveCommand<Unit, Unit> LoadCommand;
        private readonly ILocationSource _locationSource;
        private readonly IPopulationSource _populationSource;
        private readonly ICovidSource _covidSource;

        private async Task LoadAsync()
        {
            Title = "Loading ...";
            await Task.Run(async () =>
            {
                await _locationSource.LoadAsync();
                await _covidSource.LoadAsync();
                await _populationSource.LoadAsync();
            });
      
            //await (Application.Current.MainPage as MainPage).NavigateFromMenu("All");
            Title = "Loading ... Done";
        }

    }
}