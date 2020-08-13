using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using KonaAnalyzer.Data;
using KonaAnalyzer.Interfaces;
using KonaAnalyzer.Models;
using KonaAnalyzer.Services;
using PolyhydraGames.SQLite.Interfaces;
using SQLite;

namespace KonaAnalyzer.SqlData
{

    public class SQLCovidSource : BaseSource, ICovidSource
    {
        private readonly ISQLiteFactory _factory;
        public SQLCovidSource(ISQLiteFactory factory)
        {
            _factory = factory;
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

        public DateTime Yesterday => _lastDate - TimeSpan.FromDays(1);

        public DateTime RealYesterday => DateTime.Today - TimeSpan.FromDays(1);
        DateTime _lastDate;
        DateTime _earliestDate;
        public DateTime Latest => _lastDate.Date;
        public DateTime Earliest => _earliestDate.Date;


        protected override async Task UpdateItems()
        {
            var lastDay = Changes.Select(x => x.date).Distinct().OrderBy(x => x).LastOrDefault();
            if (lastDay.Date != RealYesterday)
            {
                var items = await DataExtensions.GetListFromUrlAsync<DayChange>(Configs.ChangesAddress);
                var newItems = items.Where(x => x.date > lastDay).ToList();
                Connection.InsertAll(newItems);
            }


            var ordered = Changes.OrderBy(x => x.date).Select(x => x.date).Distinct().ToList();
            _lastDate = ordered.LastOrDefault();
            _earliestDate = ordered.FirstOrDefault();
        }



        public IEnumerable<IChange> CountyChanges(string state, string county, DateTime startDay, DateTime endDay)
        {
            var stateAll = state == "All";
            var countyAll = county == "All";
            var subset = Table.Where(x => startDay <= x.date && x.date <= endDay);


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

        public IEnumerable<DayChange> Matching(string state, string county, DateTime? date)
        {
            var stateAll = state == "All";
            var countyAll = county == "All";
            var dateValue = date ?? Yesterday;
            var subset = Table.Where(x => x.date == dateValue);

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



        public IEnumerable<IChange> Changes => Table;


    }


    public class SQLCovidSource_sharedBase : CovidServiceBase, ICovidSource
    {
        private readonly ISQLiteFactory _factory;
        public SQLCovidSource_sharedBase(ISQLiteFactory factory)
        {
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






        protected override void UpdateRowSource(IEnumerable<IChange> store)
        {
            var lastRecorded = Changes.OrderBy(x => x.date).LastOrDefault()?.date ?? default;
            var newItems = store.Where(x => x.date > lastRecorded).ToList();
            Connection.InsertAll(newItems);
        }

        public override IEnumerable<IChange> Changes => Table;
    }
}
