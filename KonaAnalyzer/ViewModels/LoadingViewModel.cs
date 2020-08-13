

using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using KonaAnalyzer.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace KonaAnalyzer.ViewModels
{
    public class LoadingViewModel : BaseViewModel
    {
        private static bool _runOnce;
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
            this.WhenAnyValue(x => x.IsBusy).Select(x => !x).ToPropertyEx(this, x => x.ShowRefresh);
            var refresh = ReactiveCommand.CreateFromTask(async x =>
             {
                 await _locationSource.Reload();
                 await _covidSource.Reload();
                 await _populationSource.Reload();
             });
            refresh.InvokeCommand(LoadCommand);
            Refresh = refresh;
        }


        private void OnException(Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }

        public void OnAppearing()
        {
            if (_runOnce == false)
                LoadCommand.Execute().Subscribe();
        }
        Stopwatch _stopwatch = new Stopwatch();
        public string Version => Configs.Version;
        [Reactive] public string TimerText { get; set; }
        public bool ShowRefresh { [ObservableAsProperty] get; }
        public ReactiveCommand<Unit, Unit> LoadCommand;
        public ICommand Refresh { get; }
        private readonly ILocationSource _locationSource;
        private readonly IPopulationSource _populationSource;
        private readonly ICovidSource _covidSource;

        private async Task LoadAsync()
        {
            Title = "Loading ...";
            _runOnce = true;
            var cts = new CancellationTokenSource();


            var token = cts.Token;
            StartWatch(token);
            await Task.Run(async () =>
             {
                 await _locationSource.LoadAsync();
                 await _covidSource.LoadAsync();
                 await _populationSource.LoadAsync();
                 Title = "Loading ... Done";  
             }, cts.Token);
            cts.Cancel();
        }

        private async void StartWatch(CancellationToken token)
        {
            var watch = Stopwatch.StartNew();
            await Task.Run(() =>
            {
                while (token.IsCancellationRequested == false)
                {
                    TimerText = watch.Elapsed.ToString();
                    Thread.Sleep(200);
                    if (token.IsCancellationRequested)
                    {
                        Debug.WriteLine("cancelled");
                    }
                }
                //
            }, token);
            Debug.WriteLine("leaving");
            watch.Stop();
        }

    }
}