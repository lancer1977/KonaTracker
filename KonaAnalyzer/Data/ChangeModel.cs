using System;

namespace KonaAnalyzer.Data
{
    public class ChangeModel
    {
        public string Name { get; set; }
        public int Start { get; set; }
        public double Multiplier { get; set; }
        public int Population { get; set; }
        public DateTime StartDate { get; set; }
        public int DaysIn { get; set; }
        public DateTime ShelterInPlaceDate { get; set; }
    }
}
