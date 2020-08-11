using Xamarin.Forms;

namespace KonaAnalyzer
{
    public static class Configs
    {
        public static string AppCenterSecret = "__appCenterSecret__";
        public static string PopulationAddress = "https://raw.githubusercontent.com/lancer1977/KonaTracker/master/countyPop.csv";
        public static string ChangesAddress = "https://raw.githubusercontent.com/nytimes/covid-19-data/master/us-counties.csv"; 
        public static string CountiesAddress = "https://raw.githubusercontent.com/lancer1977/DataSeeds/master/covid/counties.json";
        //  string url = "https://raw.githubusercontent.com/nytimes/covid-19-data/master/us-counties.csv";
        public static string url = "https://raw.githubusercontent.com/lancer1977/DataSeeds/master/covid/us-counties.csv";

        public static string SyncfusionKey = "MzAxNzg0QDMxMzgyZTMyMmUzMGxqWURDSGJWTUxuWVVDcU5RcHBRWFJYWUFISTRaUERCR2o1VmJWZkZqblE9";
    }
}