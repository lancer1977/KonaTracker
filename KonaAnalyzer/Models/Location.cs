using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace KonaAnalyzer.Models
{
    public class Location
    {
        [JsonProperty("ID")]
        public int LocationId { get; set; }
        public string County { get; set; }
        public string State { get; set; }
    }
}
