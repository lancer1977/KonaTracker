//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Threading.Tasks;
//using KonaAnalyzer.Data.Model;
//using KonaAnalyzer.Data.Interface;
//using PolyhydraGames.Core.Data;

//namespace KonaAnalyzer.Services
//{
//    public class InMemoryPopulationSource : BaseSource,IPopulationSource
//    {    

//        public List<PopulationModel> Populations { get; set; } 

//        public int Population(string state, string county)
//        {
         
//            if (state == "All")
//            {
//                return Populations.Sum(x => x.Population);
//            }
//            if (string.IsNullOrEmpty(state) || string.IsNullOrEmpty(county)) return 0;
//            if (county == "All")
//            {
//                return Populations.Where(x => x.State == state).Sum(x => x.Population);
//            }

//            county = county.Replace("City", "").Replace("County", "");
//            return Populations.FirstOrDefault(x => x.State == state && x.County.Contains(county))?.Population ?? -1;

//        }

//        protected override async Task UpdateItems()
//        {
//            try
//            {
//                var items = await DataExtensions.GetListFromUrlAsync<PopulationCsv>(Configs.PopulationAddress);
//                Populations = items.Where(x => x != null).Select(x=>x.ToModel()).ToList();
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine(ex.Message); 
//            }
//        }
//    }
//}