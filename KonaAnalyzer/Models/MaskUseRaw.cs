namespace KonaAnalyzer.Models
{
    public class MaskUseRaw
    {
        public int COUNTYFP { get; set; }
        public double NEVER { get; set; }
        public double RARELY { get; set; }
        public double SOMETIMES { get; set; }
        public double FREQUENTLY { get; set; }
        public double ALWAYS { get; set; }
    }

    public class MaskUseModel
    {
        public int Fips { get; set; }
        public double Never { get; set; }
        public double Rarely { get; set; }
        public double Sometimes { get; set; }
        public double Frequently { get; set; }
        public double Always { get; set; }

        public MaskUseModel()
        {

        }

        public MaskUseModel(MaskUseRaw raw)
        {
            Fips = raw.COUNTYFP ;
            Never = raw.NEVER * 100;
            Sometimes = raw.SOMETIMES * 100;
            Rarely = raw.RARELY * 100;
            Frequently = raw.FREQUENTLY * 100;
            Always = raw.ALWAYS * 100;

        }
    }
}