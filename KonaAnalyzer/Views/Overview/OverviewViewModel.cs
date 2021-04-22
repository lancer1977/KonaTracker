using System.Collections.Generic;
using System.Threading.Tasks;
using KonaAnalyzer.Data.Interface;
using KonaAnalyzer.Data.Model;
using KonaAnalyzer.Services;
using KonaAnalyzer.ViewModels;
using ReactiveUI.Fody.Helpers;

namespace KonaAnalyzer.Views.Overview
{
    public class OverviewViewModel : BaseViewModel
    {
        [Reactive] public List<CountyChangeModel> Items { get; set; }
        [Reactive] public int Cases { get; set; }
        [Reactive] public int Deaths { get; set; }
        public OverviewViewModel(ILocationSource locationStore, ICovidSource covidstore, IMaskSource mask) : base(covidstore,locationStore,mask)
        {
            Title = "Overview";
         
        }

        public override Task OnAppearing()
        {
 
            var items = new List<CountyChangeModel>();

            foreach (var item in LocationStore.States())
            {
                var lastDay = DataStore.Latest;
                items.Add(new CountyChangeModel()
                {
                    Date = lastDay,
                    State = item,
                    Cases = DataStore.Total(item, lastDay),
                    Deaths = DataStore.Deaths(item, lastDay)
                });
            }

            Items = items;
            Deaths = DataStore.Deaths("All", null);
            Cases = DataStore.Total("All", null);
            return base.OnAppearing();
        }
    }
    
    
}