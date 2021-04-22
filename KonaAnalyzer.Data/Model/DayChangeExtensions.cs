using System;
using System.Collections.Generic;
using System.Linq;
using KonaAnalyzer.Data.Interface;

namespace KonaAnalyzer.Data.Model
{
    public static class DayChangeExtensions
    {
        //public static PopulationDto ToDto(this PopulationCsv csv, ILocationSource location)
        //{
        //    var fips = location.GetFips(csv.state, csv.county);
        //    return new PopulationDto()
        //    {
        //        Population = csv.population,
        //        Fips = fips
        //    };
        //}
        //public static PopulationModel ToModel(this PopulationCsv csv)
        //{

        //    return new PopulationModel()
        //    {
        //        Population = csv.population,
        //        County = csv.county,
        //        State = csv.state
        //    };
        //}


        public static LiteDayChange ToDayChange(this CountyChangeModel change, LocationModel location)
        {
            if (change == null)
                throw new NullReferenceException(nameof(change));
            if (location == null)
                throw new NullReferenceException(nameof(location));
            return new LiteDayChange()
            {
                Date = change.Date,
                Location = location,
                Cases = change.Cases,
                Deaths = change.Deaths,

            };
        }
        public static CountyChangeModel ToModel(this CountyChange day, LocationModel location)
        {

            return new CountyChangeModel()
            {
                Deaths = day.Deaths,
                Fips = day.Fips,
                Cases = day.Cases,
                Date = day.Date,
                State = location.State,
                County = location.County
            };
        }
        public static IEnumerable<IChange> ToModel(this IList<CountyChange> results, LocationModel location)
        {
            if (results.Any())
            {
                var first = results.First();
                return results.Select(x => x.ToModel(location));
            }

            return new List<IChange>();
        }
        public static IEnumerable<IChange> ToModel(this IList<CountyChange> results, ILocationSource locationSource)
        {
            if (results.Any())
            {
                var first = results.First();
                var location = locationSource.GetLocation(first.Fips);
                return results.Select(x => x.ToModel(location));
            }

            return new List<IChange>();
        }

        public static CountyChange ToDayChange(this CountyChangeCsv csv)
        {
            return new CountyChange()
            {
                Fips = csv.fips ?? 0,
                Date = csv.date,
                Cases = csv.cases ?? 0,
                Deaths = csv.deaths ?? 0
            };
        }
        public static CountyChange ToCountyChange(this CountyChangeCsv csv)
        {
            return new CountyChange()
            {
                Fips = csv.fips ?? 0,
                Date = csv.date,
                Cases = csv.cases ?? 0,
                Deaths = csv.deaths ?? 0
            };
        }
    }
}