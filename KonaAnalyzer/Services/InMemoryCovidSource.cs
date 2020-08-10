using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using KonaAnalyzer.Annotations;
using KonaAnalyzer.Data;
using Microsoft.AppCenter.Crashes;

namespace KonaAnalyzer.Data
{

    public class InMemoryCovidSource_old : INotifyPropertyChanged, ICovidSource
    {
        public InMemoryCovidSource_old(ILocationSource locationService)
        {
            _locationService = locationService;
            States = _locationService.States().ToList();
        }
        DateTime _lastDate;
        private bool _loaded;
        private ILocationSource _locationService;

        public DateTime Yesterday => _lastDate - TimeSpan.FromDays(1);

        public bool Loaded
        {
            get => _loaded;
            private set
            {
                _loaded = value;
                OnPropertyChanged();
            }
        }

        public DateTime MostRecent => _lastDate.Date;
        public async Task LoadAsync()
        {
            try
            {
                _changes.Clear();
                _changes.AddRange(  await DataExtensions.GetListFromUrlAsync<DayChange>(Configs.ChangesAddress));
                _lastDate = Changes.OrderBy(x => x.Date).Select(x => x.Date) .LastOrDefault(); 
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Crashes.TrackError(ex);
            }

            Loaded = true;
            //var text =  DataExtensions.GetCSV();

        }
 

        public DateTime LastDate(string state)
        {

            if (state == "All") return Changes.Select(x => x.Date).Distinct().OrderBy(x => x).LastOrDefault();
            return _changes.Where(x => x.state == state).Select(x => x.date).Distinct().OrderBy(x => x).LastOrDefault();
        }


        public List<string> Counties(string state)
        {
            //return Changes.Where(x => x.state == state).Select(x => x.county).OrderBy(x => x).Distinct().ToList();
            return _locationService.Counties(state).ToList();
        }

        public int Total(string state, string county, DateTime? date)
        {
            if (date == null) date = Yesterday;
            return _changes.Where(x => state == "All" || x.state == state)
             .Where(x => county == "All" || x.county == county)
                .Where(x => x.date == date)
                .Select(x => x.cases).Sum();
        }


        public int Deaths(string state, string county, DateTime? date)
        {
            if (date == null) date = Yesterday;
            var items = _changes.Where(x => state == "All" || x.state == state)
                .Where(x => county == "All" || x.county == county)
                .Where(x => x.date == date)
                .Select(x => x.deaths).Sum();
            return items;
        }



        public double ChangeRateByCounty(string state, string county)
        {
            throw new System.NotImplementedException();
        }

        public double ChangeRateByState(string state)
        {
            throw new System.NotImplementedException();
        }

        private bool MostRecentDay(DayChange days)
        {
            return days.date.Date == MostRecent;
        }

        private List<DayChange> _changes { get; } 
        public IEnumerable<IChange> Changes { get; } = new List<DayChange>();

        public List<string> States { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
