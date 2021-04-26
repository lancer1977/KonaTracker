 
using System.Threading.Tasks;
using KonaAnalyzer.Dapper; 
using Writer = System.Console;

namespace KonaAnalyzer.Cli
{
    public class CovidTools
    {
        KonaContextService _context;
        private DapperLocationSource _locationService;
        private DapperCovidSource _conaService;

        public CovidTools()
        {
            _context = new KonaContextService("");
            _locationService = new DapperLocationSource(_context);
            _conaService = new DapperCovidSource(_context, _locationService);
        }
        public async Task TestSources()
        {
            Writer.WriteLine("Start sources!");
      
            await _locationService.LoadAsync(); 
            await _conaService.LoadAsync();
            //await CovidSource.LoadAsync();
            Writer.WriteLine(_conaService.Total("All", _conaService.Latest));
            Writer.WriteLine("Got sources!");
        } 
    }
}
