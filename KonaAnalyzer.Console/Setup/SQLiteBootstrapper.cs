using PolyhydraGames.Core.SQLite;
using PolyhydraGames.Core.SQLite.Interfaces;
using System;
using System.IO;
using Autofac;

namespace KonaAnalyzer.Console.Setup
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



 