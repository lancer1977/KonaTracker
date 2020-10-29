using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using KonaAnalyzer.Data;
using KonaAnalyzer.Interfaces;
using KonaAnalyzer.Models;

namespace KonaAnalyzer.Services
{
    public class LocationSource : BaseSource, ILocationSource
    {


        public IEnumerable<Location> Locations { get; private set; }
        public int GetId(string state, string county)
        {
            return Locations.First(x => x.State == state && x.County == county).Fips;
        }

        public Location GetLocation(int id)
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

        public Location GetLocation(string state, string county)
        {
            return Locations.FirstOrDefault(x => x.County == county && x.State == state);
        }



        public int GetFips(string state, string county)
        {

            if (string.IsNullOrEmpty(state)) return -1;
            if (state == "All") return 0;
            if (string.IsNullOrEmpty(county)) return -1;
            Location first = null;
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
                fips = (first ?? new Location()).Fips.Round(1000);
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
                Locations = await DataExtensions.GetListFromUrlAsync<Location>(Configs.CountiesAddress, Serialize.Json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
