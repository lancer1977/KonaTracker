using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using KonaAnalyzer.Data;
using KonaAnalyzer.Data.Interface;
using KonaAnalyzer.Data.Model;
using PolyhydraGames.Core.Data;
using PolyhydraGames.Core.Interfaces;

namespace KonaAnalyzer.Dapper
{
    public class DapperLocationSource : DapperSource<LocationModel>, ILocationSource
    { 
        public IEnumerable<LocationModel> Locations => base.GetAll;
        public int GetId(string state, string county)
        {
            using var con = Factory.GetConnection();
            return con.QueryFirstOrDefault<int?>($"SELECT Fips FROM {TableName} WHERE County = @county AND State = @state",
                new { county, state }) ?? -1;
        }

        public LocationModel GetLocation(int fips)
        {
            using var con = Factory.GetConnection();
            return con.QueryFirstOrDefault<LocationModel>($"SELECT * FROM {TableName} WHERE Fips = @fips",
                new { fips });
        }

        public IEnumerable<string> Counties(string state)
        {
            using var con = Factory.GetConnection();
            return con.Query<string>($"SELECT DISTINCT County FROM {TableName} WHERE State = @State", new { state });
        }

        public IEnumerable<string> States()
        {
            using var con = Factory.GetConnection();
            return con.Query<string>($"SELECT DISTINCT State FROM {TableName}");
        }

        public LocationModel GetLocation(string state, string county)
        {
            using var con = Factory.GetConnection();
            return con.QueryFirstOrDefault<LocationModel>($"SELECT * FROM {TableName} WHERE County = @county AND State = @state", new { county, state });
        }



        public int GetFips(string state, string county)
        {
            var fips = 0;

            if (string.IsNullOrEmpty(state)) fips = -1;
            if (state == "All") return 0;
            if (string.IsNullOrEmpty(county)) fips = -1;
            if (fips == -1)
            {
                Debug.WriteLine($"State: {state} County {county}");
                return fips;
            } 

            if (county != "All")
            {
                using var con = Factory.GetConnection();
                fips = con.QueryFirstOrDefault<int?>($"SELECT Fips FROM {TableName} WHERE County = @county AND State = @state",    new { county, state }) ?? -1;
            }
            else
            {
                using var con = Factory.GetConnection();
                var first = con.QueryFirstOrDefault<int?>($"SELECT Fips FROM {TableName} WHERE State = @state",   new { county, state });

                fips = (first ?? -1).Round(1000);
            }
            if (fips == -1)
            {
                Debug.WriteLine($"Fips: {fips} State: {state} County {county}");
            }
            return fips;
        }


        public Dictionary<string, int> GetStateFipsDictionary()
        {
            Dictionary<string, int> items = new Dictionary<string, int>();

            foreach (var item in States())
            {
                var first = Locations.FirstOrDefault(x => x.State == item);
                var fipsBottom = (first ?? new LocationModel()).Fips?.Round(1000) ?? -1;
                items.Add(item, fipsBottom);
            }
            return items;
        }


        public DapperLocationSource(IDBConnectionFactory factory) : base(factory)
        {
        }

        public override async Task<List<LocationModel>> GetWebItems()
        {
            return await RawData.GetLocationModel();
        }
    }
}