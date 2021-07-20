using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using KonaAnalyzer.Data.Interface;
using KonaAnalyzer.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Xamarin.Forms;

namespace KonaAnalyzer.Views.Loading
{
    public class LoadingViewModel : BaseViewModel
    {
        public List<string> FontNames { get; } = new List<string>() { "Default", "FuturaBold", "FuturaMedium", "ProximaNovaRegular" };
        [Reactive] public string Font { get; set; } = "Default";
        private static bool _runOnce;
        public ReactiveCommand<Unit, Task> LaunchNyTimesCommand { get; set; }
        [Reactive] public string TimerText { get; set; }
        [Reactive] public DateTime LatestDay { get; set; } 
        
        public string Version => Configs.Version;

        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public bool ShowRefresh { [ObservableAsProperty] get; }
        public ReactiveCommand<Unit, Unit> LoadCommand; 
        public ICommand Refresh { get; }
        public LoadingViewModel(ILocationSource locationStore, ICovidSource covidstore, IMaskSource mask) : base(covidstore, locationStore, mask)
        {
            Title = "Loading ...";
            var generateEstimates = ReactiveCommand.CreateFromTask(GenerateEstimates).OnExecuting(x => IsBusy = x).OnException(OnException);
            this.WhenAnyValue(x => x.IsBusy).Select(x => !x).ToPropertyEx(this, x => x.ShowRefresh);
            LoadCommand = ReactiveCommand.CreateFromTask(LoadAsync).OnExecuting(x => IsBusy = x).OnException(OnException);
            Refresh = ReactiveCommand.CreateFromTask(async x =>
            {
                await DataStore.Reload();
                _runOnce = false;

            }).OnCompletion(LoadCommand);
            LaunchNyTimesCommand = ReactiveCommand.Create(async () =>
            {
                await Xamarin.Essentials.Browser.OpenAsync(Configs.nytimesAddress);
            });

            this.WhenAnyValue(x => x.Font).Subscribe(x =>
            {
                if (x == "Default") x = string.Empty;
                Application.Current.Resources["font"] = x;
            });
        }

        public async Task GenerateEstimates(CancellationToken ct)
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            StartWatch(token);
            await Task.Run(() => DataStore.GenerateEstimates(7), cts.Token);
            cts.Cancel();
        }

        private async Task LoadAsync(CancellationToken ct)
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
            await MaskStore.LoadAsync();
            await LocationStore.LoadAsync();
            await DataStore.LoadAsync();
            Title = "Loading ... Done";
            LatestDay = DataStore.Latest;
        }
   
        private async void StartWatch(CancellationToken token)
        {
            var watch = Stopwatch.StartNew();
            await Task.Run( () =>
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

        public override async Task OnAppearing()
        {
            if (_runOnce == false)
                await LoadCommand.Execute();
        }
    }
}