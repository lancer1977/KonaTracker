using Autofac;
using KonaAnalyzer.Data.SQLite;
using KonaAnalyzer.Services;

namespace KonaAnalyzer.UWP
{
    public class UWPServicesModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SQLCovidSource>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<SQLLocationSource>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<MaskUseService>().AsImplementedInterfaces().AsSelf().SingleInstance();
        }
    }
}