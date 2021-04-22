using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using KonaAnalyzer.Data.Interface;
using KonaAnalyzer.Data.Model;
using KonaAnalyzer.Services;
using KonaAnalyzer.Views;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Xamarin.Forms;

namespace KonaAnalyzer.ViewModels
{
    public class StateControlViewModel : BaseViewModel
    {
        [Reactive] public int Rank { get; set; }
        // [Reactive] public Color BackgroundColor { get; set; } = Color.LightGoldenrodYellow;
        [Reactive] public string County { get; set; }
        [Reactive] public string State { get; set; }
        [ObservableAsProperty] public int Fips { get; }
        [ObservableAsProperty] public LocationModel Location { get; }
        [ObservableAsProperty] public MaskUseModel MaskUse { get; }
        [Reactive] public int Current { get; set; }
        [Reactive] public int Dead { get; set; }
        [Reactive] public DateTime? Date { get; set; }
        [ObservableAsProperty] public int Population { get; }
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
        protected bool IsSubViewModel;
        private readonly ObservableAsPropertyHelper<string> _populationText;
        public string PopulationText => _populationText.Value;
        private readonly ObservableAsPropertyHelper<string> _dateText;

        public string DateText => _dateText.Value;
        protected async Task<Unit> UpdateValuesAsync(UpdateChanges changes)
        {
            //if (changes.Date == null || changes.Fips == -1) return Unit.Default;


            try
            {
                Debug.WriteLine($"Date{changes.Date} Fips:{changes.Fips}");
                var todayRates = await Task.Run(() => GetCurrentAndChange(changes.Fips, changes.Date));

                Current = todayRates.current;
                CurrentChange = todayRates.change;
                CurrentChangeRate = todayRates.rate * 100;
                var todayDeathRates = await Task.Run(() => GetDeathsCurrentAndChange(changes.Fips, changes.Date));
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
            return Unit.Default;
        }
        public static double GetPercentage(int numerator, int denominator)
        {
            return (((double)numerator) / ((double)denominator) * ((double)100));
        }
        public async void ItemSelected()
        {
            var nav = (Application.Current.MainPage as MasterDetailPage).Detail.Navigation;
            var statePage = new StatePage();
            statePage.ViewModel.LoadFromState(State);
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
        protected ReactiveCommand<UpdateChanges, Unit> UpdateCommand { get; }

        private (int current, int change, double rate) GetCurrentAndChange(int fips, DateTime? date)
        {
            var yesterdayDate = date - TimeSpan.FromDays(1);
            var current = DataStore.Total(fips, date);
            var yesterdayTotal = DataStore.Total(fips, yesterdayDate);
            var currentChange = current - yesterdayTotal;
            var currentChangeRate = RateChange(currentChange, yesterdayTotal);
            return (current, currentChange, currentChangeRate);

        }

        private (int current, int change, double rate) GetDeathsCurrentAndChange(int fips, DateTime? date)
        {

            var yesterdayDate = date - TimeSpan.FromDays(1);
            var current = DataStore.Deaths(fips, date);
            var yesterdayTotal = DataStore.Deaths(fips, yesterdayDate);
            var currentChange = current - yesterdayTotal;
            var currentChangeRate = RateChange(currentChange, yesterdayTotal);
            return (current, currentChange, currentChangeRate);


        }
        protected int GetTwoWeekProjectionCases((int current, int change, double rate) changes, double decay)
        {
            var total = (double)changes.current;
            var currentRate = changes.rate;
            for (var x = 0; x < 14; x++)
            {
                //currentRate += -.001;
                //if (currentRate < 0) currentRate = .001;
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

        public void Load(string county, string state, DateTime? date)
        {
            IsSubViewModel = true;
            County = county;

            Title = county == "All" ? state : county;
            Date = date;
            State = state;
            //UpdateValues(State, County, Date);
        }

        public StateControlViewModel(ICovidSource covidstore, ILocationSource locationStore,  IMaskSource mask) : base(covidstore, locationStore, mask)
        {
            this.WhenAnyValue(x => x.Date).Where(x => x != null).Select(x => x.Value.ToShortDateString()).ToProperty(this, x => x.DateText, out _dateText);
            this.WhenAnyValue(x => x.State, x => x.County, (state, county) => LocationStore.GetLocation(state, county))
                .ToPropertyEx(this, x => x.Location);

            this.WhenAnyValue(x => x.Population).Select(x => x / 1000 + "K").ToProperty(this, x => x.PopulationText, out _populationText);
            UpdateCommand = ReactiveCommand.CreateFromTask<UpdateChanges, Unit>(async (x) => await UpdateValuesAsync(x));//, this.WhenAnyValue(x => x.IsBusy).Select(x=> !x));
            UpdateCommand.IsExecuting.Subscribe(x => IsBusy = x);
            this.WhenAnyValue(x => x.Fips).Where(x => x != -1).Select(x => MaskStore.GetModel(x)).ToPropertyEx(this, x => x.MaskUse);
            this.WhenAnyValue(x => x.Location).Select(x => x?.Population ?? 0).ToPropertyEx(this, x => x.Population, -1);
            this.WhenAnyValue(x => x.Date, x => x.Location).Where(x =>
                        {
                            var (dateTime, location) = x;
                            return !(dateTime == null || location == null);
                        }).Select(x =>
                        {
                            var (dateTime, location) = x;
                            return new UpdateChanges(location.Fips ?? -1, dateTime);
                        })
                        .InvokeCommand(UpdateCommand);
            //Subscribe(async x => await UpdateValuesAsync(State, County, x));

            //this.WhenAnyValue(x => x.County).Subscribe(x => { UpdateValues(State, x, Date); });

        }






    }

    public struct UpdateChanges
    {
        public UpdateChanges(int fips, DateTime? dateTime)
        {
            Fips = fips;
            Date = dateTime;
        }
        public int Fips;
        public DateTime? Date;
    }
}