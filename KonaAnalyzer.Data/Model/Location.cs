using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KonaAnalyzer.Data.Model;
using PolyhydraGames.Core.Data;

namespace KonaAnalyzer.Data.Model
{
    public class LocationModel
    {
        public int Id { get; set; }
        public int? Fips { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        public int Population { get; set; }

    }
}
