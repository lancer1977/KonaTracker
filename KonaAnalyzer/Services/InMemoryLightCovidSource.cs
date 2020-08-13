using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using KonaAnalyzer.Data;
using KonaAnalyzer.Interfaces;
using KonaAnalyzer.Models;
using PolyhydraGames.Extensions;

namespace KonaAnalyzer.Services
{
    public class InMemoryLiteCovidSource : CovidServiceBase, ICovidSource
    {
        public InMemoryLiteCovidSource(ILocationSource locationService)
        {
            _locationService = locationService;

        }
        private readonly ILocationSource _locationService;




        private Location NoLocation = new Location() { };
        private void AddFromNoLocation(IEnumerable<DayChange> changes)
        {
            try
            {
                var localItems = changes.Where(x => x != null && (string.IsNullOrEmpty(x.county) && string.IsNullOrEmpty(x.state)));
                var converted = localItems.Select(x => ToDayChange(x, NoLocation));
                _changes.AddRange(converted);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void AddFromLocation(IEnumerable<DayChange> changes, Location location)
        {
            try
            {
                var listchanges = changes.ToList();
                if (listchanges.Any() == false) throw new Exception("changes were empty");
                if (location == null) throw new Exception("location was null");

                var localItems = changes.Where(x => x != null && x.county == location.County && x.state == location.State);
                var converted = localItems.Select(x => ToDayChange(x, location));
                _changes.AddRange(converted);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public LiteDayChange ToDayChange(DayChange change, Location location)
        {
            if (change == null)
                throw new NullReferenceException(nameof(change));
            if (location == null)
                throw new NullReferenceException(nameof(location));
            return new LiteDayChange()
            {
                date = change.date,
                Location = location,
                cases = change.cases,
                deaths = change.deaths,

            };
        }






        protected override void UpdateRowSource(IEnumerable<IChange> store)
        {
            _changes.Clear();
            //var changes = store.Select(x => new LiteDayChange()
            //{
            //    cases = x.cases,
            //    date = x.date,
            //    deaths = x.deaths
            //}).ToList();
            _locationService.Locations.ForEach(x => AddFromLocation(store.Cast<DayChange>(), x));
        }

        private List<LiteDayChange> _changes = new List<LiteDayChange>();
        public override IEnumerable<IChange> Changes => _changes;
    }

    public class InMemoryCovidSource : CovidServiceBase, ICovidSource
    {
 
         

        protected override void UpdateRowSource(IEnumerable<IChange> store)
        {
            _changes = store.Cast<DayChange>().ToList();
        }

        private List<DayChange> _changes = new List<DayChange>();
        public override IEnumerable<IChange> Changes => _changes;
    }
}
