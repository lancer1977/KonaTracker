namespace KonaAnalyzer.Models
{
    public class Location
    {
        public int Fips { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        
    }

    public class RawLocation
    {
        public int fips { get; set; }
        public string county { get; set; }
        public string state { get; set; }
    }
}
