﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KonaAnalyzer.Data;
using KonaAnalyzer.Interfaces;
using KonaAnalyzer.Models;

namespace KonaAnalyzer.Services
{
    public abstract class CovidServiceBase : BaseSource, ICovidSource
    {


        public DateTime Yesterday => _lastDate - TimeSpan.FromDays(1);

        public DateTime RealYesterday => DateTime.Today - TimeSpan.FromDays(1);
        protected DateTime _lastDate;
        protected DateTime _earliestDate;
        public DateTime Latest => _lastDate.Date;
        public DateTime Earliest => _earliestDate.Date;


        protected override async Task UpdateItems()
        {
            UpdateDates();
            if (_lastDate != RealYesterday)
            {
                UpdateRowSource(await DataExtensions.GetListFromUrlAsync<DayChange>(Configs.ChangesAddress));
            }
            UpdateDates();

        }

        private void UpdateDates()
        {
            var ordered = Changes.Select(x => x.date).Distinct().OrderBy(x => x).ToList();
            _lastDate = ordered.LastOrDefault();
            _earliestDate = ordered.FirstOrDefault();

        }
        protected abstract void UpdateRowSource(IEnumerable<IChange> store);


        public IEnumerable<IChange> CountyChanges(string state, string county, DateTime startDay, DateTime endDay)
        {
            var stateAll = state == "All";
            var countyAll = county == "All";
            var subset = Changes.Where(x => startDay <= x.date && x.date <= endDay);


            if (stateAll)
            {
                return subset;
            }
            else if (countyAll)
            {
                return subset.Where(x => x.state == state);
            }
            else
            {
                //var location = _locationService.GetLocation(state, county);
                return subset.Where(x => x.state == state && x.county == county);
            }
        }






        public int Total(string state, string county, DateTime? date)
        {
            if (date == null) date = Yesterday;
            return Matching(state, county, date)
                .Select(x => x.cases).Sum();
        }


        public int Deaths(string state, string county, DateTime? date)
        {
            if (date == null) date = Yesterday;
            var items = Matching(state, county, date).Select(x => x.deaths).Sum();
            return items;
        }

        public IEnumerable<IChange> Matching(string state, string county, DateTime? date)
        {
            var stateAll = state == "All";
            var countyAll = county == "All";
            var datevalue = date ?? Yesterday;
            var subset = Changes.Where(x => x.date == datevalue);

            if (stateAll)
            {
                return subset;
            }
            else if (countyAll)
            {
                return subset.Where(x => x.state == state);
            }
            else
            {
                //var location = _locationService.GetLocation(state, county);
                return subset.Where(x => x.state == state && x.county == county);
            }
        }



        public abstract IEnumerable<IChange> Changes { get; }


    }
}