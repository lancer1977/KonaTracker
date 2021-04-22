using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using KonaAnalyzer.Data.Interface;
using KonaAnalyzer.Data.Model;
using KonaAnalyzer.Services;
using PolyhydraGames.Core.Data;
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
                return Table.Sum(x => x.Population);
            }
            if (string.IsNullOrEmpty(state) || string.IsNullOrEmpty(county)) return 0;
            if (county == "All")
            {
                return Table.Where(x => x.State == state).Sum(x => x.Population);
            }

            county = county.Replace("City", "").Replace("County", "");
            return Table.FirstOrDefault(x => x.State == state && x.County.Contains(county))?.Population ?? -1;

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