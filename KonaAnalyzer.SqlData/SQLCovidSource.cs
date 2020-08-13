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
    public class SQLCovidSource : CovidServiceBase, ICovidSource
    {
        private readonly ISQLiteFactory _factory;
        public SQLCovidSource(ISQLiteFactory factory)
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
