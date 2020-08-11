using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using KonaAnalyzer.Data;
using ReactiveUI;
using ReactiveUI.Fody;
using ReactiveUI.Fody.Helpers;

namespace KonaAnalyzer.ViewModels
{
    public class ChangeChartViewModel : BaseViewModel
    {
        private readonly ILocationSource _locationSource;
        private readonly ICovidSource _covidSource;
        [Reactive] public IList<ChartModel> Items { get; set; }
        [Reactive] public DateTime StartDate { get; set; }
        [Reactive] public DateTime EndDate { get; set; }
        [Reactive] public string State { get; set; }
        [Reactive] public string County { get; set; }
        [Reactive] public int Maximum { get; set; }

        [Reactive] public int Interval { get; set; }
        [Reactive] public DataType DataType { get; set; }
        public List<DataType> DataTypes { get; } = new List<DataType>() { DataType.Death, DataType.Total };
        [Reactive] public bool IsUpdating { get; set; }

        public ChangeChartViewModel(ILocationSource locationSource, ICovidSource covidSource)
        {
            _locationSource = locationSource;
            _covidSource = covidSource;
            States = _locationSource.States().ToList();
            States.Insert(0, string.Empty);
            LastestDate = covidSource.Latest;
            EarliestDate = covidSource.Earliest;
            StartDate = DateTime.Today- TimeSpan.FromDays(30);
            EndDate = LastestDate;
            this.WhenAnyValue(x => x.State).Subscribe(async x =>
            {
                //await Update(x, County, StartDate, EndDate);
                var counties = _locationSource.Counties(x).ToList();
                counties.Insert(0, string.Empty);
                Counties = counties;
                ;
            }, OnException);
            this.WhenAnyValue(x => x.County).Subscribe(async x => { await Update(State, x, StartDate, EndDate); }, OnException);
            this.WhenAnyValue(x => x.StartDate).Subscribe(async x => { await Update(State, County, x, EndDate); }, OnException);
            this.WhenAnyValue(x => x.EndDate).Subscribe(async x => { await Update(State, County, StartDate, x); }, OnException);
            this.WhenAnyValue(x => x.DataType).Subscribe(async x => { await Update(State, County, StartDate, EndDate); }, OnException);


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
            if (IsUpdating ||
                //string.IsNullOrEmpty(state) || string.IsNullOrEmpty(county) ||
                startDay == default ||
                endDay == default ||  startDay >= endDay) return;
            IsUpdating = true;
            try
            {
                Debug.WriteLine($"In Update: {_updates++}");
                var change = await Task.Run(() => base.DataStore.CountyChanges(state, county, startDay, endDay));
                //State = state;
                //County = county;
                var changes = new List<ChartModel>();
                if (string.IsNullOrEmpty(state) || string.IsNullOrEmpty(county))
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
                else
                {

                }

                var sorted = change.OrderBy(x => x.date).ToList();

                for (var x = 0; x < sorted.Count; x++)
                {
                    var current = sorted[x];
                    var last = x > 0 ? sorted[x - 1] : null;
                    changes.Add(new ChartModel()
                    {
                        Date = sorted[x].date,
                        Change = last != null ?
                            DataType == DataType.Death ? current.deaths - last.deaths : current.cases - last.cases
                            : 0
                    });
                }



                Items = changes;
                var changeorder = changes.OrderBy(x => x.Change).ToList();

                var firstChange = changeorder[1].Change;
                if (firstChange == 0) firstChange = 1;
                Maximum = changeorder.Last().Change;
                Interval = (Maximum - firstChange) /10;
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
        Total
    }
}
