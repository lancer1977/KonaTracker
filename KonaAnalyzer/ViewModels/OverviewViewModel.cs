using System.Collections.Generic;
using KonaAnalyzer.Data;
using ReactiveUI.Fody.Helpers;

namespace KonaAnalyzer.ViewModels
{
    public class OverviewViewModel : BaseViewModel
    {
        [Reactive] public List<DayChange> Items { get; set; }
        [Reactive] public int Cases { get; set; }
        [Reactive] public int Deaths { get; set; }
        public OverviewViewModel()
        {
            Title = "Overview";
            var items = new List<DayChange>();

            foreach (var item in DataStore.States)
            {
                var lastDay = DataStore.Latest;
                items.Add(new DayChange()
                {
                    date = lastDay,
                    state = item,
                    cases = DataStore.Total(item, "All", lastDay),
                    deaths = DataStore.Deaths(item, "All", lastDay)
                });
            }

            Items = items;
            Deaths = DataStore.Deaths("All", "All", null);
            Cases = DataStore.Total("All", "All", null);
        }

    }
}