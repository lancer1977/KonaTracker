using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using KonaAnalyzer.Data;
using PolyhydraGames.SQLite.Interfaces;
using SQLite;

namespace KonaAnalyzer.SqlData
{

    public class SQLCovidSource : BaseSource,  ICovidSource
    {
        private readonly ISQLiteFactory _factory;
        public SQLCovidSource(ILocationSource locationService, ISQLiteFactory factory)
        {
            _locationService = locationService;
            _factory = factory;
            //            items.CreateTable<DayChange>();

        }
        private SQLiteConnection _connection;

        internal SQLiteConnection Connection
        {
            get
            {
                if (_connection != null) return _connection;
                _connection = _factory.CreateConnection();
                _connection.CreateTable<DayChange>();
                return _connection;
            }
        }

        public TableQuery<DayChange> Table => Connection.Table<DayChange>();


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
                var newItems = items.Where(x => x.date > lastDay);
                _connection.InsertAll(newItems);
            }


            var ordered = Changes.OrderBy(x => x.date).Select(x => x.date).Distinct().ToList();
            _lastDate = ordered.LastOrDefault();
            _earliestDate = ordered.FirstOrDefault();
        }


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

        public IEnumerable<IChange> CountyChanges(string state, string countyName, DateTime startDay, DateTime endDay)
        {
            return Table.Where(x => x.date > startDay && x.date <= endDay).Where(x => string.IsNullOrEmpty(state) || x.state == state).Where(x=>string.IsNullOrEmpty(countyName) || x.county == countyName);
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
            var items = Matching(state, county, date)
                .Select(x => x.deaths).Sum();
            return items;
        }

        public IEnumerable<DayChange> Matching(string state, string county, DateTime? date)
        {
            var stateAll = state == "All";
            var countyAll = county == "All";
            var datevalue = date ?? Yesterday;
            var subset = Table.Where(x => x.date == datevalue).ToList();



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
                return subset.Where(x => x.state == state);
            }
            else
            {
                var location = _locationService.GetLocation(state, county);
                return subset.Where(x => x.state == state && x.county == county);
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


        public IEnumerable<DayChange> DayChanges => Table;
        public IEnumerable<IChange> Changes => Table.ToList();

        public List<string> States { get; set; }

    }
}
