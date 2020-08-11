 
using Autofac;
using PolyhydraGames.SQLite;
using PolyhydraGames.SQLite.Interfaces;
using Xamarin.Forms.PlatformConfiguration;

namespace Covid.WPF.Setup
{
    public class SQLiteBootstrapper : Module 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
                     var path = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
            builder.Register<ISQLiteFactory>((ctx) => new SqlLiteBridge(path))
                .SingleInstance();      
        }
    }

}



 