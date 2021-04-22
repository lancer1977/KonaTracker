using System.Collections.Generic;
using System.Linq;
using KonaAnalyzer.Data.Interface;
using KonaAnalyzer.Data.Model;
using KonaAnalyzer.Services;
using PolyhydraGames.SQLite.Interfaces;
using SQLite;

namespace KonaAnalyzer.SqlData
{
    public class SQLCovidSource_sharedBase : CovidServiceBase
    {
        private readonly ISQLiteFactory _factory;
        public SQLCovidSource_sharedBase(ISQLiteFactory factory, ILocationSource d) : base(d)
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
                _connection.CreateTable<CountyChangeModel>();
                return _connection;
            }
        }



        public TableQuery<CountyChange> Table => Connection.Table<CountyChange>();






        protected override void UpdateRowSource(IEnumerable<IChange> store)
        {
            var lastRecorded = Changes.OrderBy(x => x.Date).LastOrDefault()?.Date ?? default;
            var newItems = store.Where(x => x.Date > lastRecorded).ToList();
            Connection.InsertAll(newItems);
        }

        public override IEnumerable<CountyChange> Changes => Table;
    }
}