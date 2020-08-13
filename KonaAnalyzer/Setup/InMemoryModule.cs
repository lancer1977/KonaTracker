using Autofac;
using KonaAnalyzer.Services;

namespace KonaAnalyzer.Setup
{
    public class InMemoryModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        { 
            builder.RegisterType<InMemoryLiteCovidSource>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<LocationSource>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<InMemoryPopulationSource>().AsImplementedInterfaces().AsSelf().SingleInstance();
        }
    }
}