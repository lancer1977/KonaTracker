using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using KonaAnalyzer.Data;
using Microsoft.AppCenter.Crashes;
using ReactiveUI.Fody.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(InMemoryPopulationSource))]
namespace KonaAnalyzer.Data
{
    public class InMemoryPopulationSource : IPopulationSource
    {
        string url = "https://raw.githubusercontent.com/lancer1977/KonaTracker/master/countyPop.csv";

        [Reactive] public bool Loaded { get; private set; }

        public async Task LoadAsync()
        {
            try
            {
                var items = await DataExtensions.GetListFromUrlAsync<PopulationDto>(url);
                Populations = items.Where(x => x != null).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Crashes.TrackError(ex);
            }

            Loaded = true;
            //var text =  DataExtensions.GetCSV();

        }

        public List<PopulationDto> Populations { get; set; }
        public int Population(string state, string county)
        {
            if (state == "All")
            {
                return Populations.Sum(x => x.population);
            }
            if (county == "All")
            {
                return Populations.Where(x => x.state == state).Sum(x => x.population);
            }

            county = county.Replace("City", "").Replace("County", "");
            return Populations.FirstOrDefault(x => x.state == state && x.county.Contains(county))?.population ?? -1;

        }
    }
}