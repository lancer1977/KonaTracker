using Autofac;
using KonaAnalyzer.Data.SQLite;

namespace KonaAnalyzer.UWP
{
    public class SQlModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SQLCovidSource>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<SQLLocationSource>().AsImplementedInterfaces().AsSelf().SingleInstance();
        }
    }
}