using System.Reactive;
using System.Threading.Tasks;
using KonaAnalyzer.Data;
using ReactiveUI;
using Xamarin.Forms;
using System;
using System.Diagnostics;
using KonaAnalyzer.Views;

namespace KonaAnalyzer.ViewModels
{
    public class LoadingViewModel : BaseViewModel
    {
        public LoadingViewModel()
        {
            Title = "Loading ...";

            LoadCommand = ReactiveCommand.CreateFromTask(async x=>await LoadAsync());
            LoadCommand.IsExecuting.Subscribe(x => IsBusy = x);
            LoadCommand.ThrownExceptions.Subscribe(OnException);
        }

        private void OnException(Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }

        public async void OnAppearing()
        {
            await LoadAsync();
        }
        
        public ReactiveCommand<Unit, Unit> LoadCommand;

        private async Task LoadAsync()
        {
            Title = "Loading ...";

    
            await DependencyService.Get<IPopulationSource>().LoadAsync();
            await DependencyService.Get<ICovidSource>().LoadAsync();
            //await (Application.Current.MainPage as MainPage).NavigateFromMenu("All");
            Title = "Loading ... Done";
        }

    }
}