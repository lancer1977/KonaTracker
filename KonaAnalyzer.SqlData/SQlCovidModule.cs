using Autofac;
using KonaAnalyzer.Data;

namespace KonaAnalyzer.SqlData
{
    public class SQlCovidModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SQLCovidSource>().AsImplementedInterfaces().AsSelf().SingleInstance();
        }
    }
}
