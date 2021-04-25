using KonaAnalyzer.Dapper;
using KonaAnalyzer.Data.Interface;
using Microsoft.Extensions.DependencyInjection;
using PolyhydraGames.Core.Interfaces;

namespace PolyhydraWebsite.Bootstrapper
{
    public static class KonaRegistration
    {
        public static void Register(IServiceCollection services, string connectionString)
        {
            services.AddScoped<KonaContextService>(x => new KonaContextService(connectionString));
            services.AddScoped<IDBConnectionFactory>(x => x.GetRequiredService<KonaContextService>());
            services.RegisterTypesAsScopedAndInterfaces<DapperLocationSource>();
            services.RegisterTypesAsScopedAndInterfaces<DapperCovidSource>();
            services.RegisterTypesAsScopedAndInterfaces<MaskUseSource>();

            
        }
    }
}