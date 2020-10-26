using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KonaAnalyzer.Data;
using KonaAnalyzer.Interfaces;
using KonaAnalyzer.Models;
using KonaAnalyzer.Services;
using KonaAnalyzer.Setup;
using Newtonsoft.Json;
using Writer = System.Console;

namespace KonaAnalyzer.Console
{
    public class CovidTools
    {
        private ICovidSource CovidSource => IOC.Get<ICovidSource>();
        public void StartPump()
        {
            while (true)
            {
                //TestSources().GetAwaiter().GetResult();
                  WriteCountyMap().GetAwaiter().GetResult();
                System.Console.ReadLine();
            }

        }

        public async Task TestSources()
        {
            var locationService = new LocationSource();
            await locationService.LoadAsync();

            await IOC.Get<ILocationSource>().LoadAsync();
            await IOC.Get<IPopulationSource>().LoadAsync();
            await CovidSource.LoadAsync(); 
            Writer.WriteLine(CovidSource.Total("All", "All", CovidSource.Latest));
            Writer.WriteLine("Got sources!");
        }
        public async Task WriteCountyMap()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "PolyhydraGames.counties.json"); // Library folder
            //Directory.CreateDirectory(path);
            System.Console.WriteLine(path);
            string url = "https://raw.githubusercontent.com/lancer1977/DataSeeds/master/covid/us-counties.csv";
            var source = await DataExtensions.GetListFromUrlAsync<RawLocation>(url,Serialize.CSV);
 
            //var stateId = 0;
            var locations = new List<Location>();
            foreach (var state in source.OrderBy(x=>x.fips))
            {

                if (locations.Any(x => x.Fips == state.fips)) continue;
                locations.Add(new Location()
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
    }
}
