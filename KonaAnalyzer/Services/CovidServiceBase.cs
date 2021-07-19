using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KonaAnalyzer.Data.Model;
using KonaAnalyzer.Data.Interface;
using PolyhydraGames.Core.Data;

namespace KonaAnalyzer.Services
{
    public abstract class CovidServiceBase : BaseSource, ICovidSource
    {
        private readonly ILocationSource _locationSource;
        private readonly Dictionary<string, int> _stateFips = new Dictionary<string, int>();

        protected CovidServiceBase(ILocationSource locationSource)
        {
            _locationSource = locationSource;
            this._stateFips = _locationSource.GetStateFipsDictionary();
        }

        public DateTime Yesterday => _lastDate - TimeSpan.FromDays(1);

        public DateTime RealYesterday => DateTime.Today - TimeSpan.FromDays(1);
        protected DateTime _lastDate;
        protected DateTime _earliestDate;


        public DateTime Latest => _lastDate.Date;
        public DateTime Earliest => _earliestDate.Date;
        public IEnumerable<IChange> GenerateEstimates(int days)
        {
            throw new NotImplementedException();
        }


        protected override async Task UpdateItems()
        {
            UpdateDates();
            if (_lastDate != RealYesterday)
            {
                UpdateRowSource(await DataExtensions.GetListFromUrlAsync<CountyChangeModel>(Configs.ChangesAddress));
            }
            UpdateDates();

        }

        private void UpdateDates()
        {
            var ordered = Changes.Select(x => x.Date).Distinct().OrderBy(x => x).ToList();
            _lastDate = ordered.LastOrDefault();
            _earliestDate = ordered.FirstOrDefault();

        }
        protected abstract void UpdateRowSource(IEnumerable<IChange> store);




        public abstract int Total(DateTime date);

        public abstract int Deaths(DateTime date);

        public int Total(string state, DateTime? date)
        {
            if (date == null) date = Yesterday;
            return Matching(state, "All", date).Select(x => x.Cases).Sum();
        }


        public int Deaths(string state, DateTime? date)
        {
            if (date == null) date = Yesterday;
            var items = Matching(state, "All", date).Sum(x => x.Deaths);
            return items;
        }

        public IEnumerable<CountyChange> Matching(string state, string county, DateTime? date)
        {
            var stateAll = state == "All";
            var countyAll = county == "All";
            var datevalue = date ?? Yesterday;


            var changes = Changes.Where(x => x.Date == datevalue);
            if (stateAll)
            {
                return changes;
            }


            if (countyAll)
            {
                var stateBottom = _stateFips[state];
                var stateTop = stateBottom + 1000;
                return changes.Where(x => x.Fips >= stateBottom && x.Fips < stateTop);
            }
            else
            {
                var location = _locationSource.GetLocation(state, county);
                return changes.Where(x => x.Fips == location.Fips);
            }
        }

        public IEnumerable<IChange> MatchingBetween(int fips, DateTime startDay, DateTime endDay)
        {
            var location = _locationSource.GetLocation(fips);
            if (fips == 0)
            {
                return Changes.Where(x => startDay <= x.Date && x.Date <= endDay).ToList().ToModel(location);
            }
            else if (fips % 1000 == 0)
            {
                var maxFips = fips + 1000;
                return Changes.Where(x => startDay <= x.Date && x.Date <= endDay && x.Fips < maxFips && x.Fips >= fips).ToList().ToModel(location);
            }
            else
            {
                return Changes.Where(x => startDay <= x.Date && x.Date <= endDay && x.Fips == fips).ToList().ToModel(location);
            }
        }


        public abstract IEnumerable<CountyChange> Changes { get; }

        public IEnumerable<CountyChange> Matching(int fips, DateTime? date)
        {
            var dateValue = date ?? Yesterday; 
            if (fips == 0)
            {
                return Changes.Where(x => x.Date == dateValue); ;
            }

            if (fips % 1000 == 0)
            {
                var maxfips = fips + 1000;
                return Changes.Where(x => x.Date == dateValue && x.Fips > fips && x.Fips < maxfips);
            } 
            return Changes.Where(x => x.Date == dateValue && x.Fips == fips);
        }
        public int Total(int fips, DateTime? date)
        {
            if (date == null) date = Yesterday;
            return Matching(fips, date).Select(x => x.Cases).Sum();
        }


        public int Deaths(int fips, DateTime? date)
        {
            if (date == null) date = Yesterday;
            var items = Matching(fips, date).Sum(x => x.Deaths);
            return items;
        }

    }
}