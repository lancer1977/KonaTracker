using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using KonaAnalyzer.Data;
using KonaAnalyzer.Interfaces;
using KonaAnalyzer.Models; 
namespace KonaAnalyzer.Services
{
    public abstract class CovidSourceAbstract : BaseSource, ICovidSource
    {
        protected CovidSourceAbstract(ILocationSource locationService,ISettings settings)
        {
            _locationService = locationService; 
        }
  


        private readonly ILocationSource _locationService; 
        public DateTime Yesterday => _lastDate - TimeSpan.FromDays(1);

        public DateTime RealYesterday => DateTime.Today - TimeSpan.FromDays(1);
        DateTime _lastDate;
        DateTime _earliestDate;
        public DateTime Latest => _lastDate.Date;
        public DateTime Earliest => _earliestDate.Date;


        protected override async Task UpdateItems()
        {
            States = _locationService.States().ToList();
            //Changes.Clear();
            var lastDay = Changes.Select(x => x.date).Distinct().OrderBy(x => x).LastOrDefault();
            
            if (lastDay.Date != RealYesterday)
            {
                var items = await DataExtensions.GetListFromUrlAsync<DayChange>(Configs.ChangesAddress);
                var newItems = items.Where(x => x.date > lastDay).ToList();
                try
                {
                    InsertItems(newItems);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    throw ex;
                }
              
            }


            var ordered = Changes.OrderBy(x => x.date).Select(x => x.date).Distinct().ToList();
            _lastDate = ordered.LastOrDefault();
            _earliestDate = ordered.FirstOrDefault();
        }

        protected abstract void InsertItems(IEnumerable<IChange> items);

        //public DateTime LastDate(string state)
        //{

        //    if (state == "All") return Changes.Select(x => x.date).Distinct().OrderBy(x => x).LastOrDefault();
        //    return DayChanges.Where(x => x.state == state).Select(x => x.date).Distinct().OrderBy(x => x).LastOrDefault();
        //}

        //public DateTime EarliestDate(string state)
        //{
        //    if (state == "All") return Changes.Select(x => x.date).Distinct().OrderBy(x => x).LastOrDefault();
        //    return DayChanges.Where(x => x.state == state).Select(x => x.date).Distinct().OrderBy(x => x).FirstOrDefault();
        //}

        public IEnumerable<IChange> CountyChanges(string state, string county, DateTime startDay, DateTime endDay)
        {
            var stateAll = state == "All";
            var countyAll = county == "All";
            var subset = Changes.Where(x => startDay <= x.date && x.date <= endDay);


            if (stateAll)
            {
                return subset;
            }
            else if (countyAll)
            {
                return subset.Where(x => x.state == state);
            }
            else
            {
                //var location = _locationService.GetLocation(state, county);
                return subset.Where(x => x.state == state && x.county == county);
            }
        }




        public List<string> Counties(string state)
        {
            //return Changes.Where(x => x.state == state).Select(x => x.county).OrderBy(x => x).Distinct().ToList();
            return _locationService.Counties(state).ToList();
        }

        public int Total(string state, string county, DateTime? date)
        {
            if (date == null) date = Yesterday;
            return Matching(state, county, date)
                .Select(x => x.cases).Sum();
        }


        public int Deaths(string state, string county, DateTime? date)
        {
            if (date == null) date = Yesterday;
            var items = Matching(state, county, date).Select(x => x.deaths).Sum();
            return items;
        }

        public IEnumerable<IChange> Matching(string state, string county, DateTime? date)
        {
            var stateAll = state == "All";
            var countyAll = county == "All";
            var datevalue = date ?? Yesterday;
            var subset = Changes.Where(x => x.date == datevalue);

            if (stateAll)
            {
                return subset;
            }
            else if (countyAll)
            {
                return subset.Where(x => x.state == state);
            }
            else
            {
                //var location = _locationService.GetLocation(state, county);
                return subset.Where(x => x.state == state && x.county == county);
            }
        }



        public abstract IEnumerable<IChange> Changes { get; }


        public List<string> States { get; set; }
    }
}