using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace KonaAnalyzer.ViewModels
{
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
        [Reactive] public int CurrentChangeRate { get; set; }
        [Reactive] public int DeadChange { get; set; }
        [Reactive] public int DeadChangeRate { get; set; }
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
        public string DateText => _dateText.Value;
        public ObservableCollection<StateViewModel> CountyViewModels { get; } = new ObservableCollection<StateViewModel>();
        private bool IsSubViewModel;

        public StateViewModel()
        {
            Title = "NA";
            DateUpCommand = ReactiveCommand.Create(() =>
            {
                if (Date != MaxDate) Date += TimeSpan.FromDays(1);
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
                var yesterdayDate = date - TimeSpan.FromDays(1);

                var todayRates = await GetCurrentAndChangeAsync(state, county, date);
                Current = todayRates.current;
                CurrentChange = todayRates.change;
                CurrentChangeRate = (int)(todayRates.rate * 100);
              //  var yesterday = await GetCurrentAndChange(state, county, yesterdayDate);


                var todayDeathRates = await GetDeathsCurrentAndChangeAsync(state, county, date);
                Dead = todayDeathRates.current;
                DeadChange = todayDeathRates.change;
                DeadChangeRate = (int)(todayDeathRates.rate * 100);

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
            CountyViewModels.Clear();
            foreach (var item in Counties)
            {
                if (string.IsNullOrEmpty(item) || item == "All") continue;
                var vm = new StateViewModel();
                await Task.Run(() =>
                {
                    vm.Load(item, State);
                });
                CountyViewModels.Add(vm);
            }
            LoadingCounties = false;
        }
    }
}