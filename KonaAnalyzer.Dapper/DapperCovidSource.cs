using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using KonaAnalyzer.Data;
using KonaAnalyzer.Data.Interface;
using KonaAnalyzer.Data.Model;
using PolyhydraGames.Core.Data;

namespace KonaAnalyzer.Dapper
{
    public class DapperCovidSource : DapperSource<CountyChange>, ICovidSource
    {
        public DateTime Latest { get; private set; }
        public DateTime Earliest { get; }
        private readonly ILocationSource _locationSource;

        public DapperCovidSource(KonaContextService factory, ILocationSource locationSource) : base(factory)
        {
            _locationSource = locationSource;
            Latest = GetFirst();
            Earliest = GetLast();
        }

        private readonly string StateMerge = "cc left join dbo.LocationModel loc on loc.Fips = cc.Fips";

        private DateTime GetFirst()
        {
            using var con = Factory.GetConnection();
            return con.QueryFirst<DateTime>($"SELECT Distinct Date FROM {TableName} Order By [Date] Asc");
        }

        private DateTime GetLast()
        {
            using var con = Factory.GetConnection();
            return con.QueryFirst<DateTime>($"SELECT Distinct Date FROM {TableName}  Order By Date Desc");
        }
        public int Total(string state, DateTime? date)
        {
            var dateValue = date ?? Yesterday;
            using var con = Factory.GetConnection();
            try
            {
                return con.QueryFirst<int>(  $"SELECT Sum(Cases)   FROM {TableName} {StateMerge} where State = @state and Date = @dateValue",   new { state, dateValue });
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int Deaths(string state, DateTime? date)
        {
            var dateValue = date ?? Yesterday;
            using var con = Factory.GetConnection();
            try
            {
                return con.QueryFirst<int>(
                    $"SELECT Sum(Deaths)   FROM {TableName}  {StateMerge} where State = @state and Date = @dateValue",
                    new { state, dateValue });
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        public int Total(int fips, DateTime? date)
        {
            var dateValue = date ?? Yesterday;
            using var con = Factory.GetConnection();
            try
            {
                return con.QueryFirst<int>($"SELECT Sum(Cases) FROM {TableName} where Fips = @fips and Date = @dateValue", new { fips, dateValue });
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int Deaths(int fips, DateTime? date)
        {
            var dateValue = date ?? Yesterday;
            using var con = Factory.GetConnection();
            return con.QueryFirst<int>($"SELECT Sum(Deaths) FROM {TableName} where Fips = @fips and Date = @dateValue", new { fips, dateValue });
        }
        public IEnumerable<CountyChange> Matching(int fips, DateTime? date)
        {
            var dateValue = date ?? Yesterday;
            using var con = Factory.GetConnection();
            return con.Query<CountyChange>($"SELECT * FROM {TableName} where Fips = @fips and Date = @dateValue", new { fips, dateValue });

        }

        public IEnumerable<IChange> MatchingBetween(int fips, DateTime startDay, DateTime endDay)
        {
            var changes = Matching(fips, startDay, endDay).ToList();
            return changes.ToModel(_locationSource);
        }





        public IEnumerable<CountyChange> Matching(int fips, DateTime? startDate, DateTime endDate)
        {
            using var con = Factory.GetConnection();
            return con.Query<CountyChange>($"SELECT * FROM {TableName} where Fips = @fips and Date >= @startDate and Date <= @endDate ", new { fips, startDate, endDate });

        }

        public IEnumerable<CountyChange> Matching(LocationModel model, DateTime? startDate, DateTime endDate)
        {
            if (model.Fips != null) return Matching(model.Fips.Value, startDate, endDate);
            using var con = Factory.GetConnection();
            return con.Query<CountyChange>($"SELECT * FROM {TableName}  {StateMerge} where State = @state and County = @county and Date >= @startDate and Date <= @endDate ", new { state = model.State, county = model.County, startDate, endDate });

        }
        public IEnumerable<IChange> GenerateEstimates(int days)
        {
            List<CountyChangeModel> newChanges = new List<CountyChangeModel>();
            for (var day = 0; day < days; day++)
            {

                var firstDay = Latest - TimeSpan.FromDays(7);
                foreach (var item in _locationSource.Locations)
                {
                    var sevenDayTrend = Matching(item, firstDay, Latest).ToList();
                    var (firstCases, lastCases) = sevenDayTrend.GetFirstAndLastT(x => x.Cases);
                    var casesChangeAverage = (lastCases - firstCases) / 7;
                    //cases
                    //Debug.WriteLine($"First: {firstCases} Last: {lastCases}"  );

                    var (firstDeaths, lastDeaths) = sevenDayTrend.GetFirstAndLastT(x => x.Deaths);
                    var deathChangeAverage = (lastDeaths - firstDeaths) / 7;
                    //cases
                    //Debug.WriteLine($"First: {firstDeaths} Last: {lastDeaths} Estimate: { deathChangeAverage}");

                    newChanges.Add(new CountyChangeModel()
                    {
                        Deaths = (lastDeaths + deathChangeAverage),
                        Cases = lastCases + casesChangeAverage,
                        County = item.County,
                        State = item.State,
                        //IsEstimate = true
                    });
                }
                 
            }

            return newChanges;
        }



        public override Task<List<CountyChange>> GetWebItems() => RawData.GetCountyChanges();

        public override async Task LoadAsync()
        {
            
            using var con = Factory.GetConnection();
            var lastDay = Changes.Select(x => x.Date).Distinct().OrderBy(x => x).LastOrDefault();
            if (lastDay.Date != RealYesterday)
            {
                var items = await GetWebItems();
                var newItems = items.Where(x => x.Date > lastDay).ToList();
                con.Insert(newItems);
                var ordered = items.Select(x => x.Date).OrderBy(x => x.Date).FirstOrDefault();
                Latest = ordered;
            }
        }


        public DateTime Yesterday => Latest - TimeSpan.FromDays(1);

        public DateTime RealYesterday => DateTime.Today - TimeSpan.FromDays(1);
 

        public IEnumerable<CountyChange> Changes => GetAll;

        /*
         select distinct * from dbo.CountyChange cc   right join dbo.LocationModel lm on lm.Fips = cc.Fips where lm.State = 'Virginia' order by cc.Fips 

 SELECT Sum(Cases) Total FROM CountyChange cc left join dbo.LocationModel loc on loc.Fips = cc.Fips where State = 'Alabama' and Date = '6/21/2020'
 SELECT * FROM CountyChange cc left join dbo.LocationModel loc on loc.Fips = cc.Fips where State = 'Alabama'  
  SELECT Sum(Cases) Total FROM CountyChange cc left join dbo.LocationModel loc on loc.Fips = cc.Fips where Date = '6/21/2020'
         */
    }
}