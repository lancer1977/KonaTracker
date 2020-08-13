using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class LocationSource : BaseSource, ILocationSource
    {


        public IEnumerable<Location> Locations => Table;
        public int GetId(string state, string county)
        {
            return Table.First(x => x.State == state && x.County == county).LocationId;
        }

        public Location GetLocation(int id)
        {
            return Table.First(x => x.LocationId == id);
        }

        public IEnumerable<string> Counties(string state)
        {
            return Table.Where(x => x.State == state).Select(x => x.County);
        }

        public IEnumerable<string> States()
        {
            return Table.Select(x => x.State).Distinct();
        }

        public Location GetLocation(string state, string county)
        {
            return Table.FirstOrDefault(x => x.County == county && x.State == state);
        }

        protected override async Task UpdateItems()
        {
            if (Table.Any()) return;
            try
            {
                var locations = await DataExtensions.GetListFromUrlAsync<Location>(Configs.CountiesAddress, Serialize.Json);
                Connection.InsertAll(locations);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message); 
            }
        }

        private readonly ISQLiteFactory _factory;
        public LocationSource(ISQLiteFactory factory)
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
                _connection.CreateTable<Location>();
                return _connection;
            }
        }



        private TableQuery<Location> Table => Connection.Table<Location>();
    }
}