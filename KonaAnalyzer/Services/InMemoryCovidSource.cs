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

        private List<CountyChange> _changes = new List<CountyChange>();
        public override IEnumerable<CountyChange> Changes => _changes; 
        public InMemoryCovidSource(ILocationSource locationSource) : base(locationSource)
        {
        }
    }
}