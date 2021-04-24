using Autofac;
using KonaAnalyzer.Dapper;

namespace KonaAnalyzer.Cli.Setup
{
    public class DapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder); 
            builder.RegisterType<DapperCovidSource>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<DapperLocationSource>().AsImplementedInterfaces().SingleInstance();
        }
    }
}