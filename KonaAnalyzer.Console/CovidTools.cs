using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KonaAnalyzer.Dapper;
using KonaAnalyzer.Data.Interface;
using KonaAnalyzer.Data.Model; 
using Newtonsoft.Json;
using PolyhydraGames.Core.Data;
using PolyhydraGames.Core.IOC;
using Writer = System.Console;

namespace KonaAnalyzer.Cli
{
    public class CovidTools
    {
        private ICovidSource CovidSource => IOC.Get<ICovidSource>();
 

        public async Task TestSources()
        {
            Writer.WriteLine("Start sources!");
            var locationService = IOC.Get<ILocationSource>();
            await locationService.LoadAsync();
             
            //await CovidSource.LoadAsync();
            Writer.WriteLine(CovidSource.Total("All",   CovidSource.Latest));
            Writer.WriteLine("Got sources!");
        }
        /*
        public async Task WriteCountyMap()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "PolyhydraGames.counties.json"); // Library folder
            //Directory.CreateDirectory(path);
            System.Console.WriteLine(path);
            string url = "https://raw.githubusercontent.com/lancer1977/DataSeeds/master/covid/us-counties.csv";
            var source = await DataExtensions.GetListFromUrlAsync<RawLocation>(url, Serialize.CSV);

            //var stateId = 0;
            var locations = new List<LocationModel>();
            foreach (var state in source.OrderBy(x => x.fips))
            {

                if (locations.Any(x => x.Fips == state.fips)) continue;
                locations.Add(new LocationModel()
                {
                    State = state.state,
                    County = state.county,
                    Fips = state.fips
                });
            }
            string json = JsonConvert.SerializeObject(locations);
            Writer.WriteLine(json);
            Writer.ReadKey();
            System.IO.File.WriteAllText(path, json);
        }
        */
    }
}
