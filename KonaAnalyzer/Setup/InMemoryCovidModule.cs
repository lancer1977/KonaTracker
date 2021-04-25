using Autofac;
using KonaAnalyzer.Services;

namespace KonaAnalyzer.Setup
{
    public class InMemoryCovidModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        { 
            builder.RegisterType<InMemoryCovidSource>().AsImplementedInterfaces().AsSelf().SingleInstance();
        }
    }
}