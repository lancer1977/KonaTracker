using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KonaAnalyzer.Data;
using KonaAnalyzer.Data.Interface;
using KonaAnalyzer.Data.Model;
using PolyhydraGames.Core.Data;
using PolyhydraGames.Extensions;
using PolyhydraGames.SQLite.Interfaces;
using SQLite;

namespace KonaAnalyzer.SqlData
{
    public class SQLCovidSource : SQLSource<CountyChange>, ICovidSource
    {
        private readonly ILocationSource _locationService;
        private readonly Dictionary<string, int> _stateFips = new Dictionary<string, int>();

        public IEnumerable<IChange> GenerateEstimates(int days)
        {
            List<CountyChangeModel> newChanges = new List<CountyChangeModel>();
            for (var day = 0; day < days; day++)
            {

                var firstDay = _lastDate - TimeSpan.FromDays(7);
                foreach (var item in _locationService.Locations)
                {
                    var sevenDayTrend = Matching(item.State, item.County, firstDay, _lastDate).ToList();
                    var (firstCases, lastCases) = sevenDayTrend.GetFirstAndLastT(x => x.Cases);
                    var casesChangeAverage = (lastCases - firstCases) / 7;
                    //cases
                    //Debug.WriteLine($"First: {firstCases} Last: {lastCases}"  );

                    var (firstDeaths, lastDeaths) = sevenDayTrend.GetFirstAndLastT(x => x.Deaths);
                    var deathChangeAverage = (lastDeaths - firstDeaths) / 7;
                    //cases
                    //Debug.WriteLine($"First: {firstDeaths} Last: {lastDeaths} Estimate: { deathChangeAverage}");

                    newChanges.Add(new CountyChangeModel()
                    {
                        Deaths = (lastDeaths + deathChangeAverage),
                        Cases = lastCases + casesChangeAverage,
                        County = item.County,
                        State = item.State,
                        //IsEstimate = true
                    });
                }

                _lastDate += TimeSpan.FromDays(1);
            }

            return newChanges;

        }


        public DateTime Yesterday => _lastDate - TimeSpan.FromDays(1);

        public DateTime RealYesterday => DateTime.Today - TimeSpan.FromDays(1);
        DateTime _lastDate;
        DateTime _earliestDate;

        public DateTime Latest => _lastDate.Date;
        public DateTime Earliest => _earliestDate.Date;


        protected override async Task UpdateItems()
        {
            var lastDay = Changes.Select(x => x.Date).Distinct().OrderBy(x => x).LastOrDefault();
            if (lastDay.Date != RealYesterday)
            {
                var items = await RawData.GetCountyChanges();
                var newItems = items.Where(x => x.Date > lastDay);
                Connection.InsertAll(newItems);
                var ordered = Changes.OrderBy(x => x.Date).Select(x => x.Date).Distinct().ToList();
                _lastDate = ordered.LastOrDefault();
                _earliestDate = ordered.FirstOrDefault();
            } 
        } 

        public int Total(int fips, DateTime? date)
        {
            if (date == null) date = Yesterday;
            return Matching(fips, date).Select(x => x.Cases).Sum();
        } 

        public int Deaths(int fips, DateTime? date)
        {
            if (date == null) date = Yesterday;
            var items = Matching(fips, date).Select(x => x.Deaths).Sum();
            return items;
        }

        public IEnumerable<IChange> MatchingBetween(int fips, DateTime startDay, DateTime endDay)
        {
            var location = _locationService.GetLocation(fips);
            if (fips == 0)
            {
                return Table.Where(x => startDay <= x.Date && x.Date <= endDay).ToList().ToModel(location);
            }

            if (fips % 1000 == 0)
            {
                var maxFips = fips + 1000;
                return Table.Where(x => startDay <= x.Date && x.Date <= endDay && (x.Fips < maxFips || x.Fips >= fips)).ToList().ToModel(location);
            }
            return Table.Where(x => startDay <= x.Date && x.Date <= endDay && x.Fips == fips).ToList().ToModel(location);
        }


        public IEnumerable<CountyChange> Matching(string state, string county, params DateTime[] days)
        {
            var subset = Table.Where(x => days.Contains(x.Date));
            return Matching(subset, state, county);
        }

        public int Total(DateTime date)
        {
            if (date == null) date = Yesterday;
            return Matching("All", "All", date).Select(x => x.Cases).Sum();
        }

        public int Deaths(DateTime date)
        {
            if (date == null) date = Yesterday;
            var items = Matching("All", "All", date).Sum(x => x.Deaths);
            return items;
        }


        public int Total(string state, DateTime? date)
        {
            if (date == null) date = Yesterday;
            return Matching(state, "All", date)
                .Select(x => x.Cases).Sum();
        }
        public int Deaths(string state, DateTime? date)
        {
            if (date == null) date = Yesterday;
            var items = Matching(state, "All", date).Select(x => x.Deaths).Sum();
            return items;
        }

        public IEnumerable<CountyChange> Matching(string state, string county, DateTime? date)
        {
            var dateValue = date ?? Yesterday;
            var subset = Table.Where(x => x.Date == dateValue);
            return Matching(subset, state, county);
        }

        private IEnumerable<CountyChange> Matching(TableQuery<CountyChange> changes, string state, string county)
        {
            var stateAll = state == "All";
            var countyAll = county == "All";
            if (stateAll)
            {
                return changes;
            }


            if (countyAll)
            {
                var stateBottom = _stateFips[state];
                return changes.Where(x => x.Fips >= stateBottom || x.Fips < stateBottom + 1000);
            }
            else
            {
                var location = _locationService.GetLocation(state, county);
                return changes.Where(x => x.Fips == location.Fips);
            }
        }

        public IEnumerable<CountyChange> Matching(int fips, DateTime? date)
        {
            var dateValue = date ?? Yesterday;
            var subset = Table.Where(x => x.Date == dateValue);
            return Matching(subset, fips);
        }

        private static IEnumerable<CountyChange> Matching(TableQuery<CountyChange> changes, int fips)
        {

            if (fips == 0)
            {
                return changes;
            }

            if (fips % 1000 == 0)
            {
                var maxfips = fips + 1000;
                return changes.Where(x => x.Fips > fips && x.Fips < maxfips);
            }
            //var location = _locationService.GetLocation(state, county);
            return changes.Where(x => x.Fips == fips);
        }

        public IEnumerable<CountyChange> Changes => Table;


        public SQLCovidSource(ISQLiteFactory factory, ILocationSource locationService) : base(factory)
        {
            _locationService = locationService;
            this._stateFips = locationService.GetStateFipsDictionary();
        }
    }
}
