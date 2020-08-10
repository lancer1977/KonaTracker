using System;
using System.Runtime.Serialization;
using SQLite;

namespace KonaAnalyzer.Data
{
    [DataContract]
    public class DayChange: IChange
    { 
        public DateTime date { get; set; } 

        public string county { get; set; } 
        public string state { get; set; } 
        public int cases { get; set; }
        public int deaths { get; set; }
         
        //[PrimaryKey,AutoIncrement]
        //public int Id { get; set; }
    }
     
    public class ChartModel 
    {
        public DateTime Date { get; set; }

        public int Change { get; set; }

        //[PrimaryKey,AutoIncrement]
        //public int Id { get; set; }
    }
}
