using Autofac;
using KonaAnalyzer.Data;

namespace KonaAnalyzer.SqlData
{
    public class SQlModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SQLCovidSource>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<LocationSource>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<PopulationSource>().AsImplementedInterfaces().AsSelf().SingleInstance();
        }
    }

    public class SQlCovidModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SQLCovidSource>().AsImplementedInterfaces().AsSelf().SingleInstance(); 
        }
    }
}
