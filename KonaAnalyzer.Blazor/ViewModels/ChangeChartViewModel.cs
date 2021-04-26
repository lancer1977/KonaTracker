using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using KonaAnalyzer.Data.Model;
using KonaAnalyzer.Data.Interface; 
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace KonaAnalyzer.Blazor.ViewModels
{
    public class ChangeChartViewModel : BaseViewModel
    { 
        public ICommand ToggleControls { get; }
        [Reactive] public bool ShowControls { get; set; }
        [Reactive] public string StatusText { get; set; }
        public IList<ChartModel> Items { [ObservableAsProperty] get; }
        [Reactive] public DateTime StartDate { get; set; }
        [Reactive] public DateTime EndDate { get; set; }
        [Reactive] public string State { get; set; }
        [Reactive] public string County { get; set; }
        [Reactive] public int Fips { get; set; }
        [Reactive] public int Maximum { get; set; }
        [Reactive] public int Minimum { get; set; }
        [Reactive] public bool ShowLabels { get; set; }
        [Reactive] public bool ShowMarkers { get; set; }
        [Reactive] public int Interval { get; set; }
        [Reactive] public DataType DataType { get; set; }
        public List<DataType> DataTypes { get; } = new List<DataType>() { DataType.Death, DataType.Cases, DataType.DeathPercent, DataType.CasesPercent };
        [Reactive] public bool IsUpdating { get; set; }
        [Reactive] public string LabelFormat { get; set; }
        public ReactiveCommand<ChartUpdate, IList<ChartModel>> GetItems { get; }

        public ChangeChartViewModel(ILocationSource locationStore, ICovidSourceAsync covidstore, IMaskSource mask) : base(covidstore, locationStore, mask)
        { 
            State = "All";
            County = "All";
            Fips = 0;
            States = LocationStore.States().ToList();
            States.Insert(0, "All");
            LastestDate = DataStore.Latest;
            EarliestDate = DataStore.Earliest;
            ShowControls = true;
            StartDate = DateTime.Today - TimeSpan.FromDays(30);
            EndDate = LastestDate;
            var notUpdating = this.WhenAnyValue(x => x.IsUpdating, x => !x);
            GetItems = ReactiveCommand.CreateFromTask<ChartUpdate, IList<ChartModel>>(async x => await Update(x), notUpdating);
            GetItems.IsExecuting.Subscribe(x => IsUpdating = x);
            GetItems.ToPropertyEx(this, x => x.Items);

            this.WhenAnyValue(x => x.State).Subscribe(x =>
          {
              //await Update(x, County, StartDate, EndDate);
              var counties = LocationStore.Counties(x).ToList();
              counties.Insert(0, "All");
              Counties = counties;
              County = "All";
          }, OnException);



            this.WhenAnyValue(x => x.Items).Subscribe(OnItemsChanged, OnException);
            this.WhenAnyValue(x => x.County, x => GetChartUpdate(State, x, StartDate, EndDate)).InvokeCommand(GetItems);
            this.WhenAnyValue(x => x.StartDate, x => GetChartUpdate(State, County, x, EndDate)).InvokeCommand(GetItems);
            this.WhenAnyValue(x => x.EndDate, x => GetChartUpdate(State, County, StartDate, x)).InvokeCommand(GetItems);
            this.WhenAnyValue(x => x.DataType).Select(x => GetChartUpdate(State, County, StartDate, EndDate)).InvokeCommand(GetItems);
            this.WhenAnyValue(x => x.DataType).Subscribe(x =>
            {

                switch (x)
                {
                    case DataType.Death:
                        LabelFormat = "#";
                        break;
                    case DataType.Cases:
                        LabelFormat = "#";
                        break;
                    case DataType.CasesPercent:
                        LabelFormat = "#.#'%'";
                        break;
                    case DataType.DeathPercent:
                        LabelFormat = "#.#'%'";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }, OnException);
            ToggleControls = ReactiveCommand.Create(() => ShowControls = !ShowControls);
        }

        private ChartUpdate GetChartUpdate(string state, string county, DateTime start, DateTime end)
        {
            var fips = LocationStore.GetFips(state, county);
            return new ChartUpdate(state,county,start,end,fips);
        }

        public void OnException(Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
        [Reactive] public List<string> Counties { get; set; }
        [Reactive] public List<string> States { get; set; }
        public DateTime EarliestDate { get; }
        public DateTime LastestDate { get; }
         
        public void Load(string state, string county, DateTime startDay, DateTime endDay)
        { 
            State = state;
            County = county;
            StartDate = startDay; 
            EndDate = endDay;
        }

        private int _updates; 
        private async Task<List<ChartModel>> Update(ChartUpdate update) //string state, string county, DateTime startDay, DateTime endDay)
        {
            var changes = new List<ChartModel>();
            if (IsUpdating || update.Fips == -1 || string.IsNullOrEmpty(update.State) || string.IsNullOrEmpty(update.County) || update.StartDay == default || update.EndDay == default || update.StartDay >= update.EndDay) return changes;

            try
            {
                Debug.WriteLine($"In Update: {_updates++}");
                //var change = DataStore.MatchingBetween(update.State, update.County, update.StartDay, update.EndDay);
                var change = await  DataStore.MatchingBetweenAsync(update.Fips, update.StartDay, update.EndDay);
                //State = state;
                //County = county;

                if (update.State == "All" || "All" == update.County)
                {
                    change = change.GroupBy(x => x.Date).Select(x => new CountyChangeModel()
                    {
                        Date = x.Key,
                        Deaths = x.Sum(y => y.Deaths),
                        Cases = x.Sum(y => y.Cases),
                        State = update.State,
                        County = update.County
                    });

                }

                var sorted = change.OrderBy(x => x.Date).ToList();

                for (var x = 0; x < sorted.Count; x++)
                {
                    var current = sorted[x];
                    var last = x > 0 ? sorted[x - 1] : null;
                    var localChange = 0.0;


                    if (last != null)
                    {
                        var changeCases = current.Cases - last.Cases;
                        var changeDeaths = current.Deaths - last.Deaths;
                        switch (DataType)
                        {
                            case DataType.Death:
                                localChange = changeDeaths ;
                                break;
                            case DataType.Cases:
                                localChange = changeCases;
                                break;
                            case DataType.CasesPercent:

                                localChange = 100 * ((double)changeCases / (double)current.Cases);
                                break;
                            case DataType.DeathPercent:
                                localChange = 100 * ((double)changeDeaths / (double)current.Deaths);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    changes.Add(new ChartModel()
                    {
                        Date = sorted[x].Date,
                        Change = localChange
                    });

                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

            }
            return changes;
        }

        private void OnItemsChanged(IList<ChartModel> changes)
        {
            if (changes == null || !changes.Any()) return;
            var changeorder = changes.OrderBy(x => x.Change).ToList();

            var firstChange = changeorder[1].Change;
            if (firstChange == 0) firstChange = 1;
            var maxItem = (int)changeorder.Last().Change;
            Interval = 1 + (int)((maxItem - firstChange) / 10);
            Maximum = maxItem + Interval;
            Minimum = (int)(changes.FirstOrDefault()?.Change ?? 0);
            //return Unit.Default;
        }
    }


    public enum DataType
    {
        Death,
        CasesPercent,
        DeathPercent,
        Cases
    }
}
