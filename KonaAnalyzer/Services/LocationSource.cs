using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using KonaAnalyzer.Data.Model;
using KonaAnalyzer.Data.Interface;
using PolyhydraGames.Core.Data;

namespace KonaAnalyzer.Services
{
    public class LocationSource : BaseSource, ILocationSource
    {


        public IEnumerable<LocationModel> Locations { get; private set; }
        public int GetId(string state, string county)
        {
            return Locations.First(x => x.State == state && x.County == county).Fips ?? -1;
        }

        public LocationModel GetLocation(int id)
        {
            return Locations.First(x => x.Fips == id);
        }

        public IEnumerable<string> Counties(string state)
        {
            return Locations.Where(x => x.State == state).Select(x => x.County);
        }

        public IEnumerable<string> States()
        {
            return Locations.Select(x => x.State).Distinct();
        }

        public LocationModel GetLocation(string state, string county)
        {
            return Locations.FirstOrDefault(x => x.County == county && x.State == state);
        }

        public   Dictionary<string,int> GetStateFipsDictionary( )
        {
            Dictionary<string, int> items = new Dictionary<string, int>();

            foreach (var item in States())
            {
                var first = Locations.FirstOrDefault(x => x.State == item  );
                var fipsBottom  = (first ?? new LocationModel()).Fips?.Round(1000) ?? -1;
                items.Add(item,fipsBottom);
            }
            return items;
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
                first = Locations.FirstOrDefault(x => x.State == state && x.County == county);
                fips = first?.Fips ?? -1;
            }
            else
            {
                foreach (var x in Locations)
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

        //public int GetFips(string state, string county)
        //{
        //    var getLoc = new Func<string, string, Location>((s, c) =>
        //    {
        //        return Locations.FirstOrDefault(x => x.State == s && x.County == c);
        //    });
        //    return GetFipsBase(state, county, getLoc);
        //}

        protected override async Task UpdateItems()
        {
            try
            {
                Locations = await DataExtensions.GetListFromUrlAsync<LocationModel>(Configs.CountiesAddress, Serialize.Json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
