//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using KonaAnalyzer.Data.Model;
//using KonaAnalyzer.Data.Interface;
//using PolyhydraGames.Extensions;

//namespace KonaAnalyzer.Services
//{
//    public class InMemoryLiteCovidSource : CovidServiceBase, ICovidSource
//    {
//        public InMemoryLiteCovidSource(ILocationSource locationService)
//        {
//            _locationService = locationService;

//        }
//        private readonly ILocationSource _locationService;




//        private LocationModel NoLocation = new LocationModel() { };
//        private void AddFromNoLocation(IEnumerable<CountyChangeModel> changes)
//        {
//            try
//            {
//                var localItems = changes.Where(x => x != null && (string.IsNullOrEmpty(x.County) && string.IsNullOrEmpty(x.State)));
//                var converted = localItems.Select(x => x.ToDayChange( NoLocation));
//                _changes.AddRange(converted);
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine(ex.Message);
//            }
//        }

//        private void AddFromLocation(IEnumerable<CountyChangeModel> changes, LocationModel location)
//        {
//            try
//            {
//                var listchanges = changes.ToList();
//                if (listchanges.Any() == false) throw new Exception("changes were empty");
//                if (location == null) throw new Exception("location was null");

//                var localItems = changes.Where(x => x != null && x.County == location.County && x.State == location.State);
//                var converted = localItems.Select(x => x.ToDayChange(location));
//                _changes.AddRange(converted);
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine(ex.Message);
//            }
//        }







//        protected override void UpdateRowSource(IEnumerable<IChange> store)
//        {
//            _changes.Clear();
//            //var changes = store.Select(x => new LiteDayChange()
//            //{
//            //    cases = x.cases,
//            //    date = x.date,
//            //    deaths = x.Deaths
//            //}).ToList();
//            _locationService.Locations.ForEach(x => AddFromLocation(store.Cast<CountyChangeModel>(), x));
//        }

//        private readonly List<CountyChange> _changes = new List<CountyChange>();
//        public override IEnumerable<CountyChange> Changes => _changes;
//    }
//}
