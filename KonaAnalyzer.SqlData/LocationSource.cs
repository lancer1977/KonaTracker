using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using KonaAnalyzer.Data.Model;
using KonaAnalyzer.Data.Interface;
using KonaAnalyzer.Services;
using PolyhydraGames.Core.Data;
using PolyhydraGames.SQLite.Interfaces;
using SQLite;

namespace KonaAnalyzer.SqlData
{
    public class SQLLocationSource : SQLSource<LocationModel>, ILocationSource
    {

        public Dictionary<string, int> GetStateFipsDictionary()
        {
             var items = new Dictionary<string, int>();

            foreach (var item in States())
            {
                var first = Table.FirstOrDefault(x => x.State == item);
                var fipsBottom = (first ?? new LocationModel()).Fips?.Round(1000) ?? -1;
                items.Add(item, fipsBottom);
            }
            return items;
        }
        public IEnumerable<LocationModel> Locations => Table;
        public int GetId(string state, string county)
        {
            return Table.First(x => x.State == state && x.County == county).Fips ?? -1;
        }

        public LocationModel GetLocation(int id)
        {
            if (id % 1000 == 0)
            {
                var first = Table.OrderBy(x=>x.Fips).First(x => x.Fips > id);
                first.County = "All";
                first.Fips = id;
                return first;
            } 
            return Table.First(x => x.Fips == id);
        }

        public IEnumerable<string> Counties(string state)
        {
            return Table.Where(x => x.State == state).Select(x => x.County);
        }

        public IEnumerable<string> States()
        {
            return Table.Select(x => x.State).Distinct();
        }

        public LocationModel GetLocation(string state, string county)
        {
            if (string.IsNullOrEmpty(state)) return null;
            if (state == "All")
                return new LocationModel()
                {
                    Fips = 0,
                    State = "All"
                };
            if (string.IsNullOrEmpty(county)) return null;
            if (county == "All")
            { 
                return new LocationModel()
                {
                    Fips = Locations.FirstOrDefault(x => x.State == state)?.Fips?.Round(1000) ?? 0,
                    State = state
                };
            }
            return Table.FirstOrDefault(x => x.County == county && x.State == state);
        }

        public int GetFips(string state, string county)
        {
            if (string.IsNullOrEmpty(state)) return -1;
            if (state == "All") return 0;
            if (string.IsNullOrEmpty(county)) return -1;

            LocationModel first = null;
            var fips = -1;
            if (county != "All")
            {
                first = Table.FirstOrDefault(x => x.State == state && x.County == county);
                fips = first?.Fips ?? -1;
            }
            else
            {
                foreach (var x in Table)
                {
                    if (x.State != state) continue;
                    first = x;
                    break;
                }
                fips = (first ?? new LocationModel()).Fips?.Round(1000) ?? -1;
            }

            Debug.WriteLine($"Fips: {fips} State:{state} County:{county}");
            return fips;


        }



        protected override async Task UpdateItems()
        {
            if (Table.Any()) return;
            try
            {
                var locations = await DataExtensions.GetListFromUrlAsync<LocationModel>(Configs.CountiesAddress, Serialize.Json);
                Connection.InsertAll(locations);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private readonly ISQLiteFactory _factory;
        public SQLLocationSource(ISQLiteFactory factory) : base(factory)
        {


        }

    }
}