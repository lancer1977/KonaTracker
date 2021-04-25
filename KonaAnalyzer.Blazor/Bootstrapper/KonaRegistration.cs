using KonaAnalyzer.Dapper; 
using Microsoft.Extensions.DependencyInjection;

namespace PolyhydraWebsite.Bootstrapper
{
    public static class KonaRegistration
    {
        public static void Register(IServiceCollection services, string connectionString)
        {
            services.AddScoped<KonaContextService>(x => new KonaContextService(connectionString));
            services.RegisterTypesAsScopedAndInterfaces<DapperLocationSource>();
            services.RegisterTypesAsScopedAndInterfaces<DapperCovidSource>();
            services.RegisterTypesAsScopedAndInterfaces<MaskUseSource>();
        }
    }
}