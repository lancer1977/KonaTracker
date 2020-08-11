using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using KonaAnalyzer.Data;
using KonaAnalyzer.Interfaces;
using KonaAnalyzer.Models;
using Microsoft.AppCenter.Crashes;

namespace KonaAnalyzer.Services
{
    public class LocationSource : BaseSource, ILocationSource
    {
 

        public List<Location> Locations { get; private set; }
        public int GetId(string state, string county)
        {
            return Locations.First(x => x.State == state && x.County == county).Id;
        }

        public Location GetLocation(int id)
        {
            return Locations.First(x => x.Id == id);
        }
  
        public IEnumerable<string> Counties(string state)
        {
            return Locations.Where(x => x.State == state).Select(x => x.County) ;
        }

        public IEnumerable<string> States()
        {
            return Locations.Select(x => x.State ).Distinct();
        }

        public Location GetLocation(string state, string county)
        {
            return Locations.FirstOrDefault(x => x.County == county && x.State == state);
        }

        protected override async Task UpdateItems()
        {
            try
            {
                Locations = await DataExtensions.GetListFromUrlAsync<Location>(Configs.CountiesAddress, Serialize.Json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Crashes.TrackError(ex);
            }
        }
    }
}
