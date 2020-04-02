using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace KonaAnalyzer.ViewModels
{
    public class StateViewModel : BaseViewModel
    {
        //[Reactive] public ChangeModel Model { get; set; }
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
        [Reactive] public double DeathRisk { get; set; }
        [Reactive] public double IllnessRisk { get; set; }
        [Reactive] public double TwoWeekProjectionCases { get; set; }
        [Reactive] public double TwoWeekProjectionDeaths { get; set; }
        [Reactive] public List<DateTime> Dates { get; set; } = new List<DateTime>();


        public StateViewModel()
        {
            Title = "NA";
            this.WhenAnyValue(x => x.Date).Skip(1).Subscribe(x => UpdateValues(State, County, x));
            this.WhenAnyValue(x => x.County).Subscribe(x => { UpdateValues(State, x, Date); });
            this.WhenAnyValue(x => x.State).Where(x => string.IsNullOrEmpty(x) == false).Subscribe(x =>
              {
                  Title = x;
                  PopulateCounties(x);
                  County = "All";
                  var lastDate = DataStore.LastDate(x);
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
      
                var todayRates = GetCurrentAndChange(state, county, date);
                Current = todayRates.current;
                CurrentChange = todayRates.change;
                CurrentChangeRate = todayRates.rate;
                var yesterday = GetCurrentAndChange(state, county, yesterdayDate);
                TwoWeekProjectionCases = GetTwoWeekProjectionCases(todayRates, todayRates.rate - yesterday.rate);

                var todayDeathRates = GetDeathsCurrentAndChange(state, county, date);
                Dead = todayDeathRates.current; 
                DeadChange = todayDeathRates.change; 
                DeadChangeRate = todayDeathRates.rate; 
                TwoWeekProjectionDeaths = GetTwoWeekProjectionCases(todayDeathRates, todayDeathRates.rate - GetDeathsCurrentAndChange(state, county, yesterdayDate).rate);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }


        }

        private (int current,int change, double rate) GetCurrentAndChange(string state, string county, DateTime? date)
        {
            var yesterdayDate = date - TimeSpan.FromDays(1);
            var current = DataStore.Total(state, county, date);
            var yesterdayTotal = DataStore.Total(state, county, yesterdayDate);
            var currentChange = current - yesterdayTotal;
            var currentChangeRate = RateChange(currentChange, yesterdayTotal);
            return (current, currentChange, currentChangeRate);
        }

        private (int current, int change, double rate) GetDeathsCurrentAndChange(string state, string county, DateTime? date)
        {
            var yesterdayDate = date - TimeSpan.FromDays(1);
            var current = DataStore.Deaths(state, county, date);
            var yesterdayTotal = DataStore.Deaths(state, county, yesterdayDate);
            var currentChange = current - yesterdayTotal;
            var currentChangeRate = RateChange(currentChange, yesterdayTotal);
            return (current, currentChange, currentChangeRate);
        }

        private int GetTwoWeekProjectionCases((int current, int change, double rate) changes, double decay)
        {
            var total = (double) changes.current;
            var currentRate = changes.rate;
            for (var x = 0; x < 14; x++)
            {
                currentRate += decay;
                total += (total * currentRate);

            }

            return (int) total;
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