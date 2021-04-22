using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KonaAnalyzer.Data.Model;
using PolyhydraGames.Core.Data;

namespace KonaAnalyzer.Data
{
    public static class RawData
    {
        public static async Task<List<CountyChange>> GetCountyChanges()
        {
            var results = await DataExtensions.GetListFromUrlAsync<CountyChangeCsv>(Addresses.ChangesAddress);
            return results.Select(x => x.ToCountyChange()).ToList();
        }
        public static async Task<List<LocationModel>> GetLocationModel()
        {
            List<LocationModel> models = new List<LocationModel>();
            var locationItems = await DataExtensions.GetListFromUrlAsync<RawLocation>(Addresses.CountiesAddress, Serialize.Json);
            var populationItems = await DataExtensions.GetListFromUrlAsync<RawPopulation>(Addresses.PopulationAddress);
            foreach (var item in populationItems)
            {
                item.county = item.county.Trim();
            }

            foreach (var popItem in populationItems)
            {
                var locationItem =
                    locationItems.FirstOrDefault(x => x.state == popItem.state && x.county == popItem.county);
                if (locationItem?.fips == null) continue;
                models.Add(new LocationModel()
                {
                    County = popItem.county,
                    State = popItem.state,
                    Population = popItem.population,
                    Fips = locationItem.fips  
                });
                if (locationItem != null) locationItems.Remove(locationItem);
            }

            foreach (var popItem in locationItems)
            {
                if (popItem.fips == null) continue;
                models.Add(new LocationModel()
                {
                    County = popItem.county,
                    State = popItem.state, 
                    Fips = popItem.fips
                });
            }
            return models;
        }
    }
}