using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KonaAnalyzer.Data;
using KonaAnalyzer.Interfaces;
using KonaAnalyzer.Models;

namespace KonaAnalyzer.Services
{
    public class MaskUseService : BaseSource, IMaskSource
    {
        protected override async Task UpdateItems()
        {
            UpdateRowSource(await DataExtensions.GetListFromUrlAsync<MaskUseRaw>(Configs.MaskUseAddress));


        }


        protected void UpdateRowSource(IEnumerable<MaskUseRaw> store)
        {
            _changes = store.Select(x=>new MaskUseModel(x)).ToList();
        }

        private List<MaskUseModel> _changes = new List<MaskUseModel>();
        public List<MaskUseModel> Changes => _changes;

        public MaskUseModel GetModel(int fips)
        {
            if (fips % 1000 == 0)
            {
                var maxfips = fips + 1000;
                var items = fips == 0 ? Changes : Changes.Where(x => x.Fips > fips && x.Fips < maxfips).ToList();
                if(items.Count == 0) return new MaskUseModel();
                return new MaskUseModel()
                { 
                    Frequently = items.Average(x=>x.Frequently),
                    Always = items.Average(x=>x.Always),
                    Never = items.Average(x=>x.Never),
                    Rarely =  items.Average(x=>x.Rarely),
                    Sometimes =  items.Average(x=>x.Sometimes)
                };
            }
            else
            {
                return Changes.FirstOrDefault(x => x.Fips == fips);
            }
        }
    }
    public interface IMaskSource : IDataSource
    {
          List<MaskUseModel> Changes { get; }
          MaskUseModel GetModel(int fips);
    }
}