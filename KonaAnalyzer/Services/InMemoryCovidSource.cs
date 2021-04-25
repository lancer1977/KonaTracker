using System;
using System.Collections.Generic;
using System.Linq;
using KonaAnalyzer.Data.Interface;
using KonaAnalyzer.Data.Model;

namespace KonaAnalyzer.Services
{
    public class InMemoryCovidSource : CovidServiceBase, ICovidSource
    { 
        protected override void UpdateRowSource(IEnumerable<IChange> store)
        {
            _changes = store.Cast<CountyChange>().ToList();
        }
        public override int Total(DateTime date)
        {
            if (date == null) date = Yesterday;
            return Matching("All", "All", date).Select(x => x.Cases).Sum();
        }

        public override int Deaths(DateTime date)
        {
            if (date == null) date = Yesterday;
            var items = Matching("All", "All", date).Sum(x => x.Deaths);
            return items;
        }
        private List<CountyChange> _changes = new List<CountyChange>();
        public override IEnumerable<CountyChange> Changes => _changes; 
        public InMemoryCovidSource(ILocationSource locationSource) : base(locationSource)
        {
        }
    }
}