using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using KonaAnalyzer.Data;
using KonaAnalyzer.Interfaces;
using KonaAnalyzer.Models;
using PolyhydraGames.Extensions;

namespace KonaAnalyzer.Services
{
    public class InMemoryLiteCovidSource : BaseSource, ICovidSource
    {
        public InMemoryLiteCovidSource(ILocationSource locationService)
        {
            _locationService = locationService;

        }
        private readonly ILocationSource _locationService;

        public DateTime Yesterday => _lastDate - TimeSpan.FromDays(1);


        DateTime _lastDate;
        DateTime _earliestDate;
        public DateTime Latest => _lastDate.Date;
        public DateTime Earliest => _earliestDate.Date;


        private Location NoLocation = new Location() { };
        private void AddFromNoLocation(IEnumerable<DayChange> changes)
        {
            try
            {
                var localItems = changes.Where(x => x != null && (string.IsNullOrEmpty(x.county) && string.IsNullOrEmpty(x.state)));
                var converted = localItems.Select(x => ToDayChange(x, NoLocation));
                _changes.AddRange(converted);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void AddFromLocation(IEnumerable<DayChange> changes, Location location)
        {
            try
            {
                if (location == null) throw new Exception("location was null");
                var localItems = changes.Where(x => x != null && x.county == location.County && x.state == location.State);
                var converted = localItems.Select(x => ToDayChange(x, location));
                _changes.AddRange(converted);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public LiteDayChange ToDayChange(DayChange change, Location location)
        {
            if (change == null)
                throw new NullReferenceException(nameof(change));
            if (location == null)
                throw new NullReferenceException(nameof(location));
            return new LiteDayChange()
            {
                date = change.date,
                Location = location,
                cases = change.cases,
                deaths = change.deaths,

            };
        }

        public IEnumerable<IChange> CountyChanges(string state, string countyName, DateTime startDay, DateTime endDay)
        {
            var location = _locationService.GetLocation(state, countyName);
            return _changes.Where(x => x.date > startDay && x.date <= endDay).Where(x => x.Location == location).ToList();
        }


        public DateTime LastDate(string state)
        {

            if (state == "All") return Changes.Select(x => x.date).Distinct().OrderBy(x => x).LastOrDefault();
            return _changes.Where(x => x.Location.State == state).Select(x => x.date).Distinct().OrderBy(x => x).LastOrDefault();
        }



        public List<string> Counties(string state)
        {
            //return Changes.Where(x => x.state == state).Select(x => x.county).OrderBy(x => x).Distinct().ToList();
            return _locationService.Counties(state).ToList();
        }

        public int Total(string state, string county, DateTime? date)
        {
            if (date == null) date = Yesterday;
            return Matching(state, county, date).Select(x => x.cases).Sum();
        }


        public int Deaths(string state, string county, DateTime? date)
        {
            if (date == null) date = Yesterday;
            return Matching(state, county, date).Select(x => x.deaths).Sum();
        }

        public IEnumerable<LiteDayChange> Matching(string state, string county, DateTime? date)
        {
            var stateAll = state == "All";
            var countyAll = county == "All";
            if (date == null) date = Yesterday;
            var subset = _changes.Where(x => x.date == date);
            if (stateAll && countyAll)
            {
                return subset;
            }
            else if (stateAll)
            {
                return subset;
            }
            else if (countyAll)
            {
                return subset.Where(x => x.Location.State == state);
            }
            else
            {
                var location = _locationService.GetLocation(state, county);
                return subset.Where(x => x.Location == location);
            }
        }





        private bool MostRecentDay(DayChange days)
        {
            return days.date.Date == Latest;
        }
        private List<LiteDayChange> _changes = new List<LiteDayChange>();
        public IEnumerable<IChange> Changes => _changes;
        public List<string> States { get; set; }


        protected override async Task UpdateItems()
        {
            try
            {
                States = _locationService.States().ToList();
                _changes.Clear();
                var items = await DataExtensions.GetListFromUrlAsync<DayChange>(Configs.ChangesAddress);
                _locationService.Locations.ForEach(x => AddFromLocation(items, x));
                AddFromNoLocation(items);

                var ordered = Changes.OrderBy(x => x.date).Select(x => x.date).Distinct().ToList();
                _lastDate = ordered.LastOrDefault();
                _earliestDate = ordered.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            _changes.RemoveAll(x => x == null);
            _lastDate = Changes.OrderBy(x => x.date).Select(x => x.date).LastOrDefault();
        }
    }
}
