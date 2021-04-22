using System.Collections.Generic;
using System.Threading.Tasks;
using KonaAnalyzer.Data.Interface;
using KonaAnalyzer.Data.Model;

namespace KonaAnalyzer.Dapper
{
    public class MaskUseSource : IMaskSource
    {
        public async Task LoadAsync()
        {
            
        }

        public async Task Reload()
        { 
        }

        public LoadedState LoadState { get; }
        public MaskUseModel GetModel(int fips)
        {
            return new MaskUseModel();
        }

        public List<MaskUseModel> Changes { get; } = new List<MaskUseModel>();
    }
}