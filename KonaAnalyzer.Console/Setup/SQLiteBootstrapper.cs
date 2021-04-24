using Autofac;

namespace KonaAnalyzer.Cli.Setup
{
    public class SQLiteBootstrapper : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //var dir = ApplicationData.Current.LocalFolder.Path;
            //Debug.WriteLine(dir);
            //builder.Register<ISQLiteFactory>((ctx) => new SqlLiteBridge(dir)).SingleInstance();
        }
    }
}