using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using PolyhydraGames.Extensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace KonaAnalyzer.ViewModels
{
    public enum Sort
    {
        AlphabeticalAscending,
        AlphabeticalDescending,
        TotalAscending,
        TotalDescending,

        DeadAscending,
        DeadDescending,

        PercentAscending,
        PercentDescending,
        RiskAscending,
        RiskDescending,

        PopulationAscending,
        PopulationDescending,
        MortalityAscending,
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
        private ObservableAsPropertyHelper<string> _populationText;

        public string PopulationText => _populationText.Value;

        //public ObservableCollection<StateViewModel> CountyViewModels { get; } = new ObservableCollection<StateViewModel>();
        private bool IsSubViewModel;

        public StateViewModel()
        {
            Title = "NA";
            DateUpCommand = ReactiveCommand.Create(() =>
            {
                if (Date != MaxDate) Date += TimeSpan.FromDays(1);
            });

            SortCommand = ReactiveCommand.Create<string, Unit>((x) =>
            {
                Sort = x.GetSort(Sort);
                return Unit.Default;
            });
            DateDownCommand = ReactiveCommand.Create(() => Date -= TimeSpan.FromDays(1));
            this.WhenAnyValue(x => x.Date).Skip(1).Subscribe(async x => await UpdateValuesAsync(State, County, x));
            this.WhenAnyValue(x => x.Date).Select(x => x.ToShortDateString())
                .ToProperty(this, x => x.DateText, out _dateText);
            this.WhenAnyValue(x => x.Population).Select(x => x / 1000 + "K")
                .ToProperty(this, x => x.PopulationText, out _populationText);
            this.WhenAnyValue(x => x.County).Skip(1).Subscribe(async x => { await UpdateValuesAsync(State, x, Date); });
            this.WhenAnyValue(x => x.State).Skip(1).Where(x => string.IsNullOrEmpty(x) == false).Subscribe(async x =>
            {

                if (!IsSubViewModel)
                {
                    if (State == "All")
                    {
                        PopulateStates();
                        PopulateSubStateViewModels();
                    }
                    else
                    {
                        PopulateCounties(x);
                        PopulateCountyViewModels();
                        County = "All";
                    }
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
            var sortChanged = this.WhenAnyValue(x => x.Sort).Select(x => x.GetSorter());
            this.WhenAnyValue(x => x.Sort).Select(x => x.ToFriendlyString()).ToProperty(this, x => x.SortText, out _sortText);
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


        public void LoadState(string state, bool isSubViewModel = false)
        {
            IsSubViewModel = isSubViewModel;
            State = state;
            Title = state;
        }

        public void Load(string county, string state)
        {
            IsSubViewModel = true;
            County = county;
            State = state;
            Title = county;
        }

        public async Task UpdateValuesAsync(string state, string county, DateTime? date)
        {
            if (date == default(DateTime?) || string.IsNullOrEmpty(state) || string.IsNullOrEmpty(county)) return;
            try
            {
                Population = PopulationDataStore.Population(State, County);
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
                MortalityRate = GetPercentage(Dead, Current);
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

        [Reactive] public double DeadRiskRate { get; set; }

        public static double GetPercentage(int numerator, int denominator)
        {
            return (((double)numerator) / ((double)denominator) * ((double)100));
        }

        [Reactive] public int Population { get; set; }

        public async Task UpdateValuesForSubViewModelAsync(DateTime? date)
        {
            await UpdateValuesAsync(State, County, date);
        }

        private async Task<(int current, int change, double rate)> GetCurrentAndChangeAsync(string state, string county,
            DateTime? date)
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

        private async Task<(int current, int change, double rate)> GetDeathsCurrentAndChangeAsync(string state,
            string county, DateTime? date)
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
            //counties.Insert(0, "All");
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
                await Task.Run(() => { vm.Load(item, State); });
                _sourceViewModels.Add(vm);
            }

            LoadingCounties = false;
        }

        private void PopulateStates()
        {
            var counties = DataStore.States;
            Counties = counties;
        }

        private async void PopulateSubStateViewModels()
        {
            LoadingCounties = true;
            _sourceViewModels.Clear();
            foreach (var item in Counties)
            {
                if (string.IsNullOrEmpty(item) || item == "All") continue;
                var vm = new StateViewModel();
                await Task.Run(() => { vm.LoadState(item, true); });
                _sourceViewModels.Add(vm);
            }

            LoadingCounties = false;
        }
    }

    public static class SortHelpers
    {
        public static Sort GetSort(this string command, Sort existingSort)
        {
            var existingSortString = existingSort.ToString();

            if (existingSortString.Contains(command))
            {
                return (command + (existingSortString.Contains("Ascending") ? "Descending" : "Ascending"))
                    .ToEnum<Sort>();
            }
            else
            {
                command = command + "Descending";
                var sort = command.ToEnum<Sort>();
                return sort;
            }
        }

        public static SortExpressionComparer<StateViewModel> GetSorter(this Sort x)
        {
            switch (x)
            {
                case Sort.DeadAscending: return SortExpressionComparer<StateViewModel>.Ascending(vm => vm.Dead);
                case Sort.DeadDescending: return SortExpressionComparer<StateViewModel>.Descending(vm => vm.Dead);
                case Sort.TotalAscending: return SortExpressionComparer<StateViewModel>.Ascending(vm => vm.Current);
                case Sort.TotalDescending: return SortExpressionComparer<StateViewModel>.Descending(vm => vm.Current);
                case Sort.AlphabeticalAscending: return SortExpressionComparer<StateViewModel>.Ascending(vm => vm.County);
                case Sort.AlphabeticalDescending: return SortExpressionComparer<StateViewModel>.Descending(vm => vm.County);
                case Sort.MortalityAscending: return SortExpressionComparer<StateViewModel>.Ascending(vm => vm.MortalityRate);
                case Sort.MortalityDeccending: return SortExpressionComparer<StateViewModel>.Descending(vm => vm.MortalityRate);

                case Sort.RiskAscending: return SortExpressionComparer<StateViewModel>.Ascending(vm => vm.CurrentRiskRate);
                case Sort.RiskDescending: return SortExpressionComparer<StateViewModel>.Descending(vm => vm.CurrentRiskRate);

                case Sort.PercentAscending: return SortExpressionComparer<StateViewModel>.Ascending(vm => vm.CurrentChangeRate);
                case Sort.PercentDescending: return SortExpressionComparer<StateViewModel>.Descending(vm => vm.CurrentChangeRate);

                case Sort.PopulationAscending: return SortExpressionComparer<StateViewModel>.Ascending(vm => vm.Population);
                case Sort.PopulationDescending: return SortExpressionComparer<StateViewModel>.Descending(vm => vm.Population);
          

                default: throw new ArgumentOutOfRangeException(nameof(x), x, null);
            }
        }

        public static string ToFriendlyString(this Sort x)
        {
            switch (x)
            {
                case Sort.DeadAscending: return "Deaths Asc.";
                case Sort.DeadDescending: return "Deaths Desc.";
                case Sort.TotalAscending: return "Total Asc.";
                case Sort.TotalDescending: return "Total Desc.";
                case Sort.AlphabeticalAscending: return "Alpha Asc.";
                case Sort.AlphabeticalDescending: return "Alpha Desc.";
                case Sort.MortalityAscending: return "Mort Asc.";
                case Sort.MortalityDeccending: return "Mort Desc.";
                default:
                    throw new ArgumentOutOfRangeException(nameof(x), x, null);
            }
        }
    }
}