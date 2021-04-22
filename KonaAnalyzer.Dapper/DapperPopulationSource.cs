using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using KonaAnalyzer.Data;
using KonaAnalyzer.Data.Interface;
using KonaAnalyzer.Data.Model;
using PolyhydraGames.Core.Data;
using PolyhydraGames.Core.Interfaces;

namespace KonaAnalyzer.Dapper
{
    public class DapperPopulationSource : DapperSource<PopulationDto>, IPopulationSource
    {
        private readonly ILocationSource _locationSource;

        public DapperPopulationSource(IDBConnectionFactory factory,ILocationSource locationSource) : base(factory)
        {
            _locationSource = locationSource;
        }

        public override async Task<List<PopulationDto>> GetWebItems()
        {
 
                var item = await DataExtensions.GetListFromUrlAsync<PopulationCsv>(Addresses.PopulationAddress);
                return item.Select(x => x.ToDto(_locationSource)).ToList(); 
        }



        public int Population(string state, string county)
        {
            var url = $"SELECT SUM(Population) FROM {TableName}";
            using var con = Factory.GetConnection();

            if (state == "All")
            {
                return con.QueryFirst<int>(url);
            }
            if (string.IsNullOrEmpty(state) || string.IsNullOrEmpty(county)) return 0;
            if (county == "All")
            {

                return con.QueryFirst<int>(url + " WHERE State = @state", new { state });
            }

            county = county.Replace("City", "").Replace("County", "");
            return con.QueryFirst<int>(url + " WHERE State = @state AND County = @county", new { state, county });

        }


    }
}