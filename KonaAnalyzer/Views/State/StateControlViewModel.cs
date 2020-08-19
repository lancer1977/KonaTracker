using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using KonaAnalyzer.Views;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Xamarin.Forms;

namespace KonaAnalyzer.ViewModels
{
    public class StateControlViewModel : BaseViewModel
    {
        // [Reactive] public Color BackgroundColor { get; set; } = Color.LightGoldenrodYellow;
        [Reactive] public string County { get; set; } = "All";
        [Reactive] public string State { get; set; }
        [Reactive] public int Current { get; set; }
        [Reactive] public int Dead { get; set; }
        [Reactive] public DateTime Date { get; set; }
        [Reactive] public int Population { get; set; }
        [Reactive] public int CurrentChange { get; set; }
        [Reactive] public double CurrentChangeRate { get; set; }
        [Reactive] public int DeadChange { get; set; }
        [Reactive] public double DeadChangeRate { get; set; }
        [Reactive] public double MortalityRate { get; set; }

        [Reactive] public double DeathRisk { get; set; }
        [Reactive] public double IllnessRisk { get; set; }
        [Reactive] public double TwoWeekProjectionCases { get; set; }
        [Reactive] public double TwoWeekProjectionDeaths { get; set; }
        [Reactive] public double CurrentRiskRate { get; set; }
        [Reactive] public double DeadRiskRate { get; set; }
        protected async Task UpdateValuesAsync(string state, string county, DateTime date)
        {
            if (date == default || string.IsNullOrEmpty(state) || string.IsNullOrEmpty(county)) return;
            IsBusy = true;

            try
            {
                Population = PopulationDataStore.Population(state, county);
                //var yesterdayDate = date - TimeSpan.FromDays(1);

                var todayRates = await GetCurrentAndChangeAsync(state, county, date);
                Current = todayRates.current;
                CurrentChange = todayRates.change;
                CurrentChangeRate = todayRates.rate * 100;
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
                }
                // BackgroundColor = Color.Default;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            IsBusy = false;



        }
        public static double GetPercentage(int numerator, int denominator)
        {
            return (((double)numerator) / ((double)denominator) * ((double)100));
        }
        public async void ItemSelected()
        {
            var nav = (Application.Current.MainPage as MasterDetailPage).Detail.Navigation;
            var statePage = new StatePage();
            statePage.ViewModel.Load(State);
            await nav.PushAsync(statePage);


        }
        public async void CountySelected()
        {
            var nav = (Application.Current.MainPage as MasterDetailPage).Detail.Navigation;
            var statePage = new ChangeChartPage();
            var startDay = DateTime.Today - TimeSpan.FromDays(30);
            var endDay = DataStore.Latest;
            statePage.ViewModel.Load(State, County, startDay, endDay);
            await nav.PushAsync(statePage);
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

        protected int GetTwoWeekProjectionCases((int current, int change, double rate) changes, double decay)
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

        public void Load(string county, string state, DateTime date)
        {
            IsSubViewModel = true;
            County = county;
            State = state;
            Title = county == "All" ? state : county;
            Date = date;

        }
        protected bool IsSubViewModel;
        private readonly ObservableAsPropertyHelper<string> _populationText;
        public string PopulationText => _populationText.Value;
        private readonly ObservableAsPropertyHelper<string> _dateText;

        public string DateText => _dateText.Value;
        public StateControlViewModel()
        {
            this.WhenAnyValue(x => x.Date).Subscribe(async x => await UpdateValuesAsync(State, County, x));
            this.WhenAnyValue(x => x.Date).Select(x => x.ToShortDateString()).ToProperty(this, x => x.DateText, out _dateText);
            this.WhenAnyValue(x => x.Population).Select(x => x / 1000 + "K").ToProperty(this, x => x.PopulationText, out _populationText);
            this.WhenAnyValue(x => x.County).Subscribe(async x => { await UpdateValuesAsync(State, x, Date); });
            this.WhenAnyValue(x => x.State).Where(x => string.IsNullOrEmpty(x) == false)
                .Subscribe(async (x) => await OnStateUpdatedAsync(x));
        }

        public virtual async Task OnStateUpdatedAsync(string state)
        {
            await UpdateValuesAsync(state, County, Date);
        }


    }
}