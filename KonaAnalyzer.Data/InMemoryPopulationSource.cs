using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace KonaAnalyzer.Data
{
    public class InMemoryPopulationSource :  IPopulationSource, INotifyPropertyChanged
    {
        string url = "https://raw.githubusercontent.com/lancer1977/KonaTracker/master/countyPop.csv"; 

        public void Load()
        {
            try
            {
                var items = DataExtensions.GetListFromUrl<PopulationDto>(url);
                Populations = items.Where(x => x != null).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message); 
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
            return Populations.FirstOrDefault(x => x.state == state &&  x.county.Contains(county)  )?.population ?? -1;

        }

        private bool _loaded;

        public bool Loaded
        {
            get => _loaded;
            set
            {
                if (_loaded == value) return;
                _loaded = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}