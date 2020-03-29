using System;
using System.Collections.Generic;
using System.Linq;
using KonaAnalyzer.Data;
using Xamarin.Forms;

[assembly: Dependency(typeof(InMemoryCovidSource))]
namespace KonaAnalyzer.Data
{
    public class InMemoryCovidSource : ICovidSource
    {
        string url = "https://raw.githubusercontent.com/nytimes/covid-19-data/master/us-counties.csv";
        DateTime _lastDate;
        public DateTime Yesterday => _lastDate - TimeSpan.FromDays(1);

        public DateTime MostRecent => _lastDate.Date;
        public void Load()
        {
            //var text =  DataExtensions.GetCSV();
            Changes = DataExtensions.GetListFromUrl<DayChange>(url);
            _lastDate = Changes.OrderBy(x => x.date).Select(x => x.date).LastOrDefault();//?? (DateTime.Today - TimeSpan.FromDays(1));
            States = Changes.Select(x => x.state).Distinct().OrderBy(x => x).ToList();
        }

        public int GetPopulation(string state)
        {
            string url =
                "api.census.gov/data/2019/pep/population?get=COUNTY,DATE_CODE,DATE_DESC,DENSITY,POP,NAME,STATE&for=region:*&key=YOUR_KEY";
            var censusData = DataExtensions.GetStringFromUrl(url);
            return 0;
        }

        public DateTime LastDate(string state)
        {
            return Changes.Where(x => x.state == state).Select(x => x.date).Distinct().OrderBy(x => x).LastOrDefault();
        }


        public List<string> Counties(string state)
        {
            return Changes.Where(x => x.state == state).Select(x => x.county).Distinct().ToList();
        }

        public int Total(string state, string county, DateTime? date)
        {
            if (date == null) date = Yesterday;
            return Changes.Where(x => x.state == state)
                .Where(x => county == "All" || x.county == county)
                .Where(x=>x.date == date)
                .Select(x => x.cases).Sum();
        }

 
        public int Deaths(string state, string county,DateTime? date)
        {
            if (date == null) date = Yesterday;
            var items = Changes.Where(x => x.state == state)
                .Where(x=> county == "All" || x.county == county)
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
        public List<DayChange> Changes { get; private set; }

        public List<string> States { get; set; }
    }
}
