using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows.Input;
using KonaAnalyzer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace KonaAnalyzer.ViewModels
{
    public class StateViewModel : BaseViewModel
    {
        [Reactive] public ChangeModel Model { get; set; }
        [Reactive] public List<string> Counties { get; set; }
        [Reactive] public string County { get; set; }
        [Reactive] public string State { get; set; }
        [Reactive] public int Current { get; set; }
        [Reactive] public int Dead { get; set; }
        [Reactive] public DateTime Date { get; set; } = DateTime.Today;

        [Reactive] public int CurrentChange { get; set; }
        [Reactive] public double CurrentChangeRate { get; set; }
        [Reactive] public int DeadChange { get; set; }
        [Reactive] public double DeadChangeRate { get; set; }
        [Reactive] public List<DateTime> Dates { get; set; } = new List<DateTime>();

        public StateViewModel()
        {

            this.WhenAnyValue(x => x.Date).Skip(1).Subscribe(x => UpdateValues(State, County, x));
            this.WhenAnyValue(x => x.County).Subscribe(x => { UpdateValues(State, x, Date); });
            this.WhenAnyValue(x => x.State).Where(x => string.IsNullOrEmpty(x) == false).Subscribe(x =>
              {
                  Title = x;
                  PopulateCounties(x);
                  County = "All";
                  var lastDate = DataStore.LastDate(x) ?? DateTime.Today;
                  var dateRange = new List<DateTime>();
                  for (var y = 0; y < 30; y++)
                  {
                      dateRange.Add(lastDate - TimeSpan.FromDays(y));
                  }
                  Dates = dateRange;
                  Date = lastDate;

                  UpdateValues(x, County, Date);
              });
        }
        public void LoadState(string state)
        {
            State = state;
        }

        public void UpdateValues(string state, string county, DateTime? date)
        {
            if (date == default(DateTime)) return;
            try
            {
                var yesterdayDate = date - TimeSpan.FromDays(1);
                Current = DataStore.Total(state, county, date);
                var yesterdayTotal = DataStore.Total(state, county, yesterdayDate);
                CurrentChange = Current - yesterdayTotal;

                CurrentChangeRate = RateChange(CurrentChange, yesterdayTotal);


                Dead = DataStore.Deaths(state, county, date);
                var yesterdayDeathTotal = DataStore.Deaths(state, county, yesterdayDate);
                DeadChange = Dead - yesterdayDeathTotal;
                DeadChangeRate = RateChange(DeadChange, yesterdayDeathTotal);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }


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
    }
}