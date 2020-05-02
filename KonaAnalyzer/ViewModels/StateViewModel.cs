using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace KonaAnalyzer.ViewModels
{
    public enum Sort
    {
        AlphabeticalAccending,
        AlphabeticalDecending,
     
        TotalAccending,
        TotalDescending,
        DeadAccending,
        DeadDescending,
        MortalityAccending,
        MortalityDeccending,
    }
    public class StateViewModel : BaseViewModel
    {
        [Reactive] public bool LoadingCounties { get; set; }
        //[Reactive] public ChangeModel Model { get; set; }
        [Reactive] public List<string> Counties { get; set; }
        [Reactive] public string County { get; set; } = "All";
        [Reactive] public string State { get; set; }
        [Reactive] public int Current { get; set; }
        [Reactive] public int Dead { get; set; }
        [Reactive] public DateTime Date { get; set; } = DateTime.Today;

        [Reactive] public int CurrentChange { get; set; }
        [Reactive] public double CurrentChangeRate { get; set; }
        [Reactive] public int DeadChange { get; set; }
        [Reactive] public double DeadChangeRate { get; set; }
        [Reactive] public double MortalityRate { get; set; }
        
        [Reactive] public double DeathRisk { get; set; }
        [Reactive] public double IllnessRisk { get; set; }
        [Reactive] public double TwoWeekProjectionCases { get; set; }
        [Reactive] public double TwoWeekProjectionDeaths { get; set; }
        [Reactive] public List<DateTime> Dates { get; set; } = new List<DateTime>();

        [Reactive] public DateTime MaxDate { get; set; } = DateTime.MaxValue;
        [Reactive] public DateTime MinDate { get; set; } = DateTime.MinValue;
        private readonly ObservableAsPropertyHelper<string> _dateText;
        public ICommand DateUpCommand { get; }
        public ICommand DateDownCommand { get; }
        public ICommand SortCommand { get; }
        [Reactive] public Sort Sort { get; set; }
        public string DateText => _dateText.Value;
        private readonly SourceList<StateViewModel> _sourceViewModels;
        private readonly ReadOnlyObservableCollection<StateViewModel> _countyViewModels;
        public IEnumerable<StateViewModel> CountyViewModels => _countyViewModels;

        private ObservableAsPropertyHelper<string> _sortText;

        public string SortText => _sortText.Value;
        //public ObservableCollection<StateViewModel> CountyViewModels { get; } = new ObservableCollection<StateViewModel>();
        private bool IsSubViewModel; 
        public StateViewModel()
        {
            Title = "NA";
            DateUpCommand = ReactiveCommand.Create(() =>
            {
                if (Date != MaxDate) Date += TimeSpan.FromDays(1);
            });
     
            SortCommand = ReactiveCommand.Create(() =>
            {
                Sort = Sort == Sort.MortalityDeccending ? (Sort)0 : Sort + 1;
            });
            DateDownCommand = ReactiveCommand.Create(() => Date -= TimeSpan.FromDays(1));
            this.WhenAnyValue(x => x.Date).Skip(1).Subscribe(async x => await UpdateValuesAsync(State, County, x));
            this.WhenAnyValue(x => x.Date).Select(x => x.ToShortDateString()).ToProperty(this, x => x.DateText, out _dateText);

            this.WhenAnyValue(x => x.County).Skip(1).Subscribe(async x => { await UpdateValuesAsync(State, x, Date); });
            this.WhenAnyValue(x => x.State).Skip(1).Where(x => string.IsNullOrEmpty(x) == false).Subscribe(async x =>
              {
                  Title = x;
                  if ( !IsSubViewModel)
                  {
                      PopulateCounties(x);
                      PopulateCountyViewModels();
                      County = "All";
                  }

                  var lastDate = DataStore.LastDate(x);
                  var dateRange = new List<DateTime>();
                  for (var y = 0; y < 30; y++)
                  {
                      dateRange.Add(lastDate - TimeSpan.FromDays(y));
                  }
                  Dates = dateRange;
                  MinDate = dateRange.Last();
                  MaxDate = dateRange.First();
                  Date = lastDate;

                  await UpdateValuesAsync(x, County, Date);
              });
            var sortChanged = this.WhenAnyValue(x => x.Sort).Select(x =>
            {
                switch (x)
                {
                    case Sort.DeadAccending: return SortExpressionComparer<StateViewModel>.Ascending(vm=>vm.Dead);
                    case Sort.DeadDescending: return   SortExpressionComparer<StateViewModel>.Descending(vm => vm.Dead);
                    case Sort.TotalAccending: return  SortExpressionComparer<StateViewModel>.Ascending(vm => vm.Current);
                    case Sort.TotalDescending: return  SortExpressionComparer<StateViewModel>.Descending(vm => vm.Current);
                    case Sort.AlphabeticalAccending: return SortExpressionComparer<StateViewModel>.Ascending(vm => vm.County);
                    case Sort.AlphabeticalDecending: return SortExpressionComparer<StateViewModel>.Descending(vm => vm.County);

                    case Sort.MortalityAccending: return SortExpressionComparer<StateViewModel>.Ascending(vm => vm.MortalityRate);
                    case Sort.MortalityDeccending: return SortExpressionComparer<StateViewModel>.Descending(vm => vm.MortalityRate);

                    default:
                        throw new ArgumentOutOfRangeException(nameof(x), x, null);
                }
            });
             this.WhenAnyValue(x => x.Sort).Select(x =>
            {
                switch (x)
                {
                    case Sort.DeadAccending: return "Deaths Asc.";
                    case Sort.DeadDescending: return "Deaths Desc.";
                    case Sort.TotalAccending: return "Total Asc.";
                    case Sort.TotalDescending: return "Total Desc.";
                    case Sort.AlphabeticalAccending: return "Alpha Asc.";
                    case Sort.AlphabeticalDecending: return "Alpha Desc.";
                    case Sort.MortalityAccending: return "Mort Asc.";
                    case Sort.MortalityDeccending: return "Mort Desc.";
                    default:
                        throw new ArgumentOutOfRangeException(nameof(x), x, null);
                }
            }).ToProperty(this,x=>x.SortText,out _sortText);
            _sourceViewModels = new SourceList<StateViewModel>();
            var myOperation = _sourceViewModels.Connect()
                //.Filter(trade => trade.Status == TradeStatus.Live)
                //.Transform(trade => new TradeProxy(trade))
                .Sort(sortChanged)
                //.ObserveOnDispatcher()
                .Bind(out _countyViewModels)
                .DisposeMany()
                .Subscribe();
        }
        public void LoadState(string state)
        {
            State = state;
        }
        public void Load(string county, string state)
        {
            IsSubViewModel = true;
            County = county;
            State = state;
        }
        public async Task UpdateValuesAsync(string state, string county, DateTime? date)
        {
            if (date == default(DateTime?) || string.IsNullOrEmpty(state) || string.IsNullOrEmpty(county)) return;
            try
            {
                Population = PopulationDataStore.Population(State, County) / 1000;
                var yesterdayDate = date - TimeSpan.FromDays(1);

                var todayRates = await GetCurrentAndChangeAsync(state, county, date);
                Current = todayRates.current;
                CurrentChange = todayRates.change;
                CurrentChangeRate = todayRates.rate * 100;
              //  var yesterday = await GetCurrentAndChange(state, county, yesterdayDate);


                var todayDeathRates = await GetDeathsCurrentAndChangeAsync(state, county, date);
                Dead = todayDeathRates.current;
                DeadChange = todayDeathRates.change;
                DeadChangeRate = todayDeathRates.rate * 100;

                DeadRiskRate = GetPercentage(Dead, Population);
                CurrentRiskRate = GetPercentage(Current, Population);
                MortalityRate = GetPercentage(Dead,Current);
                if (IsSubViewModel == false)
                {
                    TwoWeekProjectionCases = GetTwoWeekProjectionCases(todayRates, todayRates.rate);
                    TwoWeekProjectionDeaths = GetTwoWeekProjectionCases(todayDeathRates, todayDeathRates.rate);
                    foreach (var item in CountyViewModels)
                    {
                        item.Date = date.Value;

                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }


        }

        [Reactive] public double CurrentRiskRate { get; set; }

        [Reactive]  public double DeadRiskRate { get; set; }

        public static double GetPercentage(int numerator, int denominator)
        {
             return(((double)numerator) / ((double)denominator) * ((double)100));
        }

        [Reactive] public int Population { get; set; }

        public async Task UpdateValuesForSubViewModelAsync(DateTime? date)
        {
            await UpdateValuesAsync(State, County, date);
        }
        private async Task<(int current, int change, double rate)> GetCurrentAndChangeAsync(string state, string county, DateTime? date)
        {
            return await Task.Run(() =>
            {
                var yesterdayDate = date - TimeSpan.FromDays(1);
                var current = DataStore.Total(state, county, date);
                var yesterdayTotal = DataStore.Total(state, county, yesterdayDate);
                var currentChange = current - yesterdayTotal;
                var currentChangeRate = RateChange(currentChange, yesterdayTotal);
                return (current, currentChange, currentChangeRate);
            });

        }

        private async Task<(int current, int change, double rate)> GetDeathsCurrentAndChangeAsync(string state, string county, DateTime? date)
        {
            return await Task.Run(() =>
            {
                var yesterdayDate = date - TimeSpan.FromDays(1);
            var current = DataStore.Deaths(state, county, date);
            var yesterdayTotal = DataStore.Deaths(state, county, yesterdayDate);
            var currentChange = current - yesterdayTotal;
            var currentChangeRate = RateChange(currentChange, yesterdayTotal);
            return (current, currentChange, currentChangeRate);
            });
        }

        private int GetTwoWeekProjectionCases((int current, int change, double rate) changes, double decay)
        {
            var total = (double)changes.current;
            var currentRate = changes.rate;
            for (var x = 0; x < 14; x++)
            {
                currentRate += -.005;
                total += (total * currentRate);
                Console.WriteLine($"Day {x}: {total}");
            }

            return (int)total;
        }

        private double RateChange(int change, int yesterday)
        {
            if (yesterday == 0) return 0d;
            return Math.Round((double)change / yesterday, 3);
        }

        private void PopulateCounties(string state)
        {
            var counties = DataStore.Counties(state);
            counties.Insert(0, "All");
            Counties = counties;
        }

        private async void PopulateCountyViewModels()
        {
            LoadingCounties = true;
            _sourceViewModels.Clear();
            foreach (var item in Counties)
            {
                if (string.IsNullOrEmpty(item) || item == "All") continue;
                var vm = new StateViewModel();
                await Task.Run(() =>
                {
                    vm.Load(item, State);
                });
                _sourceViewModels.Add(vm);
            }
            LoadingCounties = false;
        }
    }
}