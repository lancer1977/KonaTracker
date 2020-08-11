namespace KonaAnalyzer.Interfaces
{
    public interface IPopulationSource : IDataSource
    {
        int Population(string state, string county);
       
    }
}