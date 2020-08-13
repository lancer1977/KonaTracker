using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using KonaAnalyzer.Data;
using KonaAnalyzer.Interfaces;
using KonaAnalyzer.Models;
using ReactiveUI;
using ReactiveUI.Fody;
using ReactiveUI.Fody.Helpers;

namespace KonaAnalyzer.ViewModels
{
    public class ChangeChartViewModel : BaseViewModel
    {
        private readonly ILocationSource _locationSource;
        private readonly ICovidSource _covidSource;
        public ICommand ToggleControls { get; }
        [Reactive] public bool ShowControls { get; set; }
        [Reactive] public IList<ChartModel> Items { get; set; }
        [Reactive] public DateTime StartDate { get; set; }
        [Reactive] public DateTime EndDate { get; set; }
        [Reactive] public string State { get; set; }
        [Reactive] public string County { get; set; }
        [Reactive] public int Maximum { get; set; }
        [Reactive] public bool ShowLabels { get; set; }
        [Reactive] public bool ShowMarkers { get; set; }
        [Reactive] public int Interval { get; set; }
        [Reactive] public DataType DataType { get; set; }
        public List<DataType> DataTypes { get; } = new List<DataType>() { DataType.Death, DataType.Cases, DataType.DeathPercent, DataType.CasesPercent };
        [Reactive] public bool IsUpdating { get; set; }
        [Reactive] public string LabelFormat { get; set; }

        public ChangeChartViewModel(ILocationSource locationSource, ICovidSource covidSource)
        {
            _locationSource = locationSource;
            _covidSource = covidSource;
            State = "All";
            County = "All";
            States = _locationSource.States().ToList();
            States.Insert(0, "All");
            LastestDate = covidSource.Latest;
            EarliestDate = covidSource.Earliest;
            ShowControls = true;
            StartDate = DateTime.Today - TimeSpan.FromDays(30);
            EndDate = LastestDate;
            this.WhenAnyValue(x => x.State).Subscribe(async x =>
            {
                //await Update(x, County, StartDate, EndDate);
                var counties = _locationSource.Counties(x).ToList();
                counties.Insert(0, "All");
                Counties = counties;
                County = "All";
            }, OnException);
            this.WhenAnyValue(x => x.County).Subscribe(async x => { await Update(State, x, StartDate, EndDate); }, OnException);
            this.WhenAnyValue(x => x.StartDate).Subscribe(async x => { await Update(State, County, x, EndDate); }, OnException);
            this.WhenAnyValue(x => x.EndDate).Subscribe(async x => { await Update(State, County, StartDate, x); }, OnException);
            this.WhenAnyValue(x => x.DataType).Subscribe(async x => { await Update(State, County, StartDate, EndDate); }, OnException);
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
        private async Task Update(string state, string county, DateTime startDay, DateTime endDay)
        {
            if (county == null) county = "All";
            if (IsUpdating ||
                //string.IsNullOrEmpty(state) || string.IsNullOrEmpty(county) ||
                startDay == default ||
                endDay == default || startDay >= endDay) return;
            IsUpdating = true;
            try
            {
                Debug.WriteLine($"In Update: {_updates++}");
                var change = await Task.Run(() => base.DataStore.CountyChanges(state, county, startDay, endDay));
                //State = state;
                //County = county;
                var changes = new List<ChartModel>();
                if (state == "All" || "All" == county)
                {
                    change = change.GroupBy(x => x.date).Select(x => new DayChange()
                    {
                        date = x.Key,
                        deaths = x.Sum(y => y.deaths),
                        cases = x.Sum(y => y.cases),
                        state = state,
                        county = county
                    });

                }

                var sorted = change.OrderBy(x => x.date).ToList();

                for (var x = 0; x < sorted.Count; x++)
                {
                    var current = sorted[x];
                    var last = x > 0 ? sorted[x - 1] : null;
                    var localChange = 0.0;


                    if (last != null)
                    {
                        var changeCases = current.cases - last.cases;
                        var changeDeaths = current.deaths - last.deaths;
                        switch (DataType)
                        {
                            case DataType.Death:
                                localChange = changeDeaths;
                                break;
                            case DataType.Cases:
                                localChange = changeCases;
                                break;
                            case DataType.CasesPercent:

                                localChange = 100 * ((double)changeCases / (double)current.cases);
                                break;
                            case DataType.DeathPercent:
                                localChange = 100 * ((double)changeDeaths / (double)current.deaths);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    changes.Add(new ChartModel()
                    {
                        Date = sorted[x].date,
                        Change = localChange
                    });

                }

                Items = changes;
                var changeorder = changes.OrderBy(x => x.Change).ToList();

                var firstChange = changeorder[1].Change;
                if (firstChange == 0) firstChange = 1;
                Maximum = 1 + (int)changeorder.Last().Change;
                Interval = 1 + (int)((Maximum - firstChange) / 10);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

            }

            IsUpdating = false;
            //var maximum = 
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
