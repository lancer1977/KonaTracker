using System.Reactive;
using System.Threading.Tasks;
using KonaAnalyzer.Data;
using ReactiveUI;
using Xamarin.Forms;
using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using KonaAnalyzer.Views;
using System.Windows.Input;
using KonaAnalyzer.Interfaces;
using ReactiveUI.Fody.Helpers;

namespace KonaAnalyzer.ViewModels
{
    public class LoadingViewModel : BaseViewModel
    {
        public LoadingViewModel(ICovidSource covidSource, IPopulationSource populationSource, ILocationSource locationSource)
        {
            Title = "Loading ...";

            LoadCommand = ReactiveCommand.CreateFromTask(async x => await LoadAsync());
            LoadCommand.IsExecuting.Subscribe(x =>
            {
                IsBusy = x;
                Debug.WriteLine($"Busy {IsBusy} sent:{x}");
            });
            LoadCommand.ThrownExceptions.Subscribe(OnException);
            _locationSource = locationSource;
            _populationSource = populationSource;
            _covidSource = covidSource;
            Refresh = ReactiveCommand.CreateFromTask(async x =>
            {
                await _locationSource.Reload();
                await _covidSource.Reload();
                await _populationSource.Reload();
            }, this.WhenAnyValue(x => x.IsBusy, x => !x));
        }


        private void OnException(Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }

        public void OnAppearing()
        {
            LoadCommand.Execute().Subscribe();
        }
        Stopwatch _stopwatch = new Stopwatch();
        [Reactive] public string TimerText { get; set; }
        public ReactiveCommand<Unit, Unit> LoadCommand;
        public ICommand Refresh { get; }
        private readonly ILocationSource _locationSource;
        private readonly IPopulationSource _populationSource;
        private readonly ICovidSource _covidSource;

        private async Task LoadAsync()
        {
            Title = "Loading ...";
            var cts = new CancellationTokenSource();

            var watch = Stopwatch.StartNew();
            Task.Run(() =>
            {
                while (true)
                {
                    TimerText = watch.Elapsed.ToString();
                    Thread.Sleep(200);
                }
            }, cts.Token);
            watch.Start();
            await Task.Run(async () =>
            {
                await _locationSource.LoadAsync();
                await _covidSource.LoadAsync();
                await _populationSource.LoadAsync();
            });
            watch.Stop();
            cts.Cancel();
            //await (Application.Current.MainPage as MainPage).NavigateFromMenu("All");
            Title = "Loading ... Done";
        }

    }
}