

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using KonaAnalyzer.Interfaces;
using KonaAnalyzer.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Xamarin.Forms;

namespace KonaAnalyzer.ViewModels
{
    public class LoadingViewModel : BaseViewModel
    {
        public List<string> FontNames { get; } = new List<string>() { "Default", "FuturaBold", "FuturaMedium", "ProximaNovaRegular" };
        [Reactive] public string Font { get; set; } = "Default";
        private static bool _runOnce;


        public ReactiveCommand<Unit, Task> LaunchNyTimesCommand { get; set; }
        [Reactive] public string TimerText { get; set; }
        [Reactive] public DateTime LatestDay { get; set; }


        Stopwatch _stopwatch = new Stopwatch();
        public string Version => Configs.Version;

        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public bool ShowRefresh { [ObservableAsProperty] get; }
        public ReactiveCommand<Unit, Unit> LoadCommand;
        public ICommand AddEstimateCommand { get; }
        public ICommand Refresh { get; }
        private readonly ILocationSource _locationSource;
        private readonly IMaskSource _maskSource;
        private readonly IPopulationSource _populationSource;
        private readonly ICovidSource _covidSource;
        public LoadingViewModel(ICovidSource covidSource, IPopulationSource populationSource, ILocationSource locationSource, IMaskSource maskSource)
        {
            Title = "Loading ...";


            _locationSource = locationSource;
            _maskSource = maskSource;
            _populationSource = populationSource;
            _covidSource = covidSource;
            var generateEstimates = ReactiveCommand.CreateFromTask(async () => await GenerateEstimates());
            generateEstimates.IsExecuting.Subscribe(x =>
            {
                IsBusy = x;
            });
            generateEstimates.ThrownExceptions.Subscribe(OnException);
            //AddEstimateCommand = generateEstimates;
            this.WhenAnyValue(x => x.IsBusy).Select(x => !x).ToPropertyEx(this, x => x.ShowRefresh);
            LoadCommand = ReactiveCommand.CreateFromTask(async x => await LoadAsync());
            LoadCommand.IsExecuting.Subscribe(x =>
            {
                IsBusy = x;
                Debug.WriteLine($"Busy {IsBusy} sent:{x}");
            });
            LoadCommand.ThrownExceptions.Subscribe(OnException);
            var refresh = ReactiveCommand.CreateFromTask(async x =>
            {
                _runOnce = false;

            });
            refresh.InvokeCommand(LoadCommand);
            LaunchNyTimesCommand = ReactiveCommand.Create(async () =>
            {
                await Xamarin.Essentials.Browser.OpenAsync(Configs.nytimesAddress);
            });
            this.WhenAnyValue(x => x.Font).Subscribe(x =>
            {
                if (x == "Default") x = string.Empty;
                Application.Current.Resources["font"] = x;
            });
            Refresh = refresh;
        }

        public async Task GenerateEstimates()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            StartWatch(token);
            await Task.Run(async () =>
            {
                _covidSource.GenerateEstimates(7);
            }, cts.Token);
            cts.Cancel();
        }

        private async Task LoadAsync()
        {
            Title = "Loading ...";
            _runOnce = true;
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            StartWatch(token);
            await Task.Run(async () =>
            {
                await LoadServices();
            }, cts.Token);
            cts.Cancel();
        }

        private async Task LoadServices()
        {
            await _maskSource.LoadAsync();

            await _locationSource.LoadAsync();
            await _covidSource.LoadAsync();
            await _populationSource.LoadAsync();
            Title = "Loading ... Done";
            LatestDay = _covidSource.Latest;
        }
        private async Task LoadServicesParallel()
        {
            var tasks = new List<Task>();
            tasks.Add(_maskSource.LoadAsync());

            tasks.Add(_locationSource.LoadAsync());
            tasks.Add(_covidSource.LoadAsync());
            tasks.Add(_populationSource.LoadAsync());
            tasks.ForEach(x => x.Start());
            Task.WaitAll(tasks.ToArray());
            Title = "Loading ... Done";
            LatestDay = _covidSource.Latest;
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
                }
                //
            }, token);
            if (token.IsCancellationRequested)
            {
                Debug.WriteLine("cancelled");
            }
            Debug.WriteLine("leaving");
            watch.Stop();
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
    }
}