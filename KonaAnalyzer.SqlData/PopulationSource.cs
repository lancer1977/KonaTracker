using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using KonaAnalyzer.Data;
using KonaAnalyzer.Interfaces;
using KonaAnalyzer.Models;
using KonaAnalyzer.Services;
using Microsoft.AppCenter.Crashes;
using PolyhydraGames.SQLite.Interfaces;
using SQLite;

namespace KonaAnalyzer.SqlData
{
    public class PopulationSource : BaseSource, IPopulationSource
    {


        public int Population(string state, string county)
        {
            if (state == "All")
            {
                return Table.Sum(x => x.population);
            }
            if (county == "All")
            {
                return Table.Where(x => x.state == state).Sum(x => x.population);
            }

            county = county.Replace("City", "").Replace("County", "");
            return Table.FirstOrDefault(x => x.state == state && x.county.Contains(county))?.population ?? -1;

        }

        protected override async Task UpdateItems()
        {
            if (Table.Any()) return;
            try
            {
                var items = await DataExtensions.GetListFromUrlAsync<PopulationDto>(Configs.PopulationAddress);
                Connection.InsertAll(items.Where(x => x != null));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Crashes.TrackError(ex);
            }
        }

        private readonly ISQLiteFactory _factory;
        public PopulationSource(ISQLiteFactory factory)
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
                _connection.CreateTable<PopulationDto>();
                return _connection;
            }
        }



        private TableQuery<PopulationDto> Table => Connection.Table<PopulationDto>();
    }
}