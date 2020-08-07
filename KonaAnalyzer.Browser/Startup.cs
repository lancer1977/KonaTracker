using KonaAnalyzert.Browser;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace KonaAnalyzer.Browser
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<KonaAnalyzer.Silverlight.App>("app");
        }
    }
}
