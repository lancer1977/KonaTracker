using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using KonaAnalyzer.Annotations;
using Microsoft.AppCenter.Crashes;

namespace KonaAnalyzer.Data
{
    public class InMemoryLiteCovidSource : BaseSource, ICovidSource
    {
        public InMemoryLiteCovidSource(ILocationSource locationService)
        {
            _locationService = locationService;
        }
        DateTime _lastDate;
        private bool _loaded;
        private ILocationSource _locationService;

        public DateTime Yesterday => _lastDate - TimeSpan.FromDays(1);

        public bool Loaded
        {
            get => _loaded;
            private set
            {
                _loaded = value;
                OnPropertyChanged();
            }
        }

        public DateTime MostRecent => _lastDate.Date;
  
        private Location NoLocation = new Location() { };
        private void AddFromNoLocation(IEnumerable<DayChange> changes )
        {
            try
            { 
                var localItems = changes.Where(x => x != null && (string.IsNullOrEmpty(x.county) && string.IsNullOrEmpty(x.state)  ));
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
                Date = change.date,
                Location = location,
                Cases = change.cases,
                Deaths = change.deaths,

            };
        }

        public DateTime LastDate(string state)
        {

            if (state == "All") return Changes.Select(x => x.Date).Distinct().OrderBy(x => x).LastOrDefault();
            return _changes.Where(x => x.Location.State == state).Select(x => x.Date).Distinct().OrderBy(x => x).LastOrDefault();
        }


        public List<string> Counties(string state)
        {
            //return Changes.Where(x => x.state == state).Select(x => x.county).OrderBy(x => x).Distinct().ToList();
            return _locationService.Counties(state).ToList();
        }

        public int Total(string state, string county, DateTime? date)
        {
            if (date == null) date = Yesterday;
            return Matching(state, county, date).Select(x => x.Cases).Sum();  
        }


        public int Deaths(string state, string county, DateTime? date)
        {
            if (date == null) date = Yesterday;
            return Matching(state, county, date).Select(x => x.Deaths).Sum(); 
        }

        public IEnumerable<LiteDayChange> Matching(string state, string county, DateTime? date)
        {
            var stateAll = state == "All"  ;
            var countyAll = county == "All";
            if (date == null) date = Yesterday;
            var subset = _changes.Where(x => x.Date == date);
            if (stateAll && countyAll)
            {
                return subset;
            }
            else if (stateAll)
            {
                return subset;
            }
            else if ( countyAll)
            {
                return subset.Where(x=> x.Location.State == state);
            }
            else
            {
                var location = _locationService.GetLocation(state, county);
                return subset.Where(x => x.Location == location) ; 
            }
        }



        public double ChangeRateByCounty(string state, string county)
        {
            throw new System.NotImplementedException();
        }

        public double ChangeRateByState(string state)
        {
            throw new System.NotImplementedException();
        }

        private bool MostRecentDay(DayChange days)
        {
            return days.date.Date == MostRecent;
        }
        private List<LiteDayChange> _changes= new List<LiteDayChange>();
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
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Crashes.TrackError(ex);
            }

            _changes.RemoveAll(x => x == null);
            _lastDate = Changes.OrderBy(x => x.Date).Select(x => x.Date).LastOrDefault();
        }
    }
}
