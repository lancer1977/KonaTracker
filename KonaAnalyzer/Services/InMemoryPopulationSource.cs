﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks; 
using Microsoft.AppCenter.Crashes; 

namespace KonaAnalyzer.Data
{
    public class InMemoryPopulationSource : BaseSource,IPopulationSource
    {    

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

        protected override async Task UpdateItems()
        {
            try
            {
                var items = await DataExtensions.GetListFromUrlAsync<PopulationDto>(Configs.PopulationAddress);
                Populations = items.Where(x => x != null).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Crashes.TrackError(ex);
            }
        }
    }
}