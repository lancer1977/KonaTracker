using System.Collections.Generic;
using System.Threading.Tasks;
using KonaAnalyzer.Data.Interface;
using KonaAnalyzer.Data.Model; 

namespace KonaAnalyzer.Blazor.Pages
{

    public abstract class BaseViewModel 
    {
        protected BaseViewModel(ICovidSourceAsync datastore, ILocationSource locationStore, IMaskSource maskSource)
        {
            DataStore = datastore;
            LocationStore = locationStore;
            MaskStore = maskSource;
        }

        public IMaskSource MaskStore { get; }
        public ICovidSourceAsync DataStore { get; }
        public ILocationSource LocationStore { get; }
        public bool IsBusy { get; set; }
        public string Title { get; set; }
 
    }
    public class OverviewViewModel : BaseViewModel
    {
         public List<CountyChangeModel> Items { get; set; } = new List<CountyChangeModel>();
         public int Cases { get; set; }
         public int Deaths { get; set; }
        public OverviewViewModel(ILocationSource locationStore, ICovidSourceAsync covidstore, IMaskSource mask) : base(covidstore, locationStore, mask)
        {
            Title = "Overview";

        }

        public   async Task OnAppearing()
        {

            var items = new List<CountyChangeModel>();
            //await DataStore.LoadAsync();
            var states = LocationStore.States();
            var lastDay = DataStore.Latest;
            foreach (var item in states)
            { 
                items.Add(new CountyChangeModel()
                {
                    Date = lastDay,
                    State = item,
                    Cases = await DataStore.TotalAsync(item, lastDay),
                    Deaths = await DataStore.DeathsAsync(item, lastDay)
                });
            }

            Items = items;
            Deaths = await DataStore.DeathsAsync( lastDay);
            Cases = await DataStore.TotalAsync( lastDay); 
        }
    }
}