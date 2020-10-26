using System.Collections.Generic;
using System.Linq;
using KonaAnalyzer.Data;
using KonaAnalyzer.Interfaces;
using KonaAnalyzer.Models;

namespace KonaAnalyzer.Services
{
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