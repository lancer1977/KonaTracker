using System.Collections.Generic;
using KonaAnalyzer.Data.Model;

namespace KonaAnalyzer.Data.Interface
{
    public interface IMaskSource : IDataSource
    {
        List<MaskUseModel> Changes { get; }
        MaskUseModel GetModel(int fips);
    }
}