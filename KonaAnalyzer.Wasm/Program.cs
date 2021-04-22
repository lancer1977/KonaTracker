using Microsoft.Extensions.Logging;
using KonaAnalyzer.Data.Interface;
using KonaAnalyzer.Setup;
using PolyhydraGames.Core.Data;
using Uno.UI.Wasm;

namespace KonaAnalyzer.Wasm
{
    public class Program
    {
        static int Main(string[] args)
        {
            //ConfigureFilters(Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory);
            HttpService.GetHandler = () => new WasmHttpHandler();

            IOC.Instance.Setup(new[] { typeof(ServicesModule), typeof(ViewModelModule), typeof(InMemoryCovidModule) });
            Windows.UI.Xaml.Application.Start(param => new KonaAnalyzer.UWP.App());

            return 0;
        }

    }
}
