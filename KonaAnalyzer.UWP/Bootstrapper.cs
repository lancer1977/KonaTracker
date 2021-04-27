using System;
using System.IO;
using Autofac;
using KonaAnalyzer.Setup;
using PolyhydraGames.SQLite;
using PolyhydraGames.SQLite.Interfaces;

namespace KonaAnalyzer.UWP
{
    public class Bootstrapper : Module
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

        public static void Initialize()
        {
            IOC.Instance.Setup(new[] { typeof(ServicesModule), typeof(ViewModelModule), typeof(Bootstrapper), typeof(SQlModule) });
        }
    }
}

