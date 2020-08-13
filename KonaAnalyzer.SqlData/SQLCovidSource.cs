using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using KonaAnalyzer.Data;
using KonaAnalyzer.Interfaces;
using KonaAnalyzer.Models;
using KonaAnalyzer.Services;
using PolyhydraGames.SQLite.Interfaces;
using SQLite;

namespace KonaAnalyzer.SqlData
{
    public class SQLCovidSource : CovidSourceAbstract, ICovidSource
    {
        private readonly ISQLiteFactory _factory; 

        public SQLCovidSource(ILocationSource locationService, ISQLiteFactory factory,ISettings settings) : base(locationService,   settings)
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



        private TableQuery<DayChange> Table => Connection.Table<DayChange>();


        protected override void InsertItems(IEnumerable<IChange> items)
        {
            Connection.InsertAll(items.Cast<DayChange>());
        }

        public override IEnumerable<IChange> Changes => Table;


    }
}
