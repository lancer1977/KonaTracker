using System;
using System.IO;
using Autofac;
using KonaAnalyzer.Setup;
using KonaAnalyzer.SqlData;
using PolyhydraGames.SQLite;
using PolyhydraGames.SQLite.Interfaces;

namespace KonaAnalyzer.iOS.Setup
{
    public class Bootstrapper : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder

            builder.Register<ISQLiteFactory>((ctx) => new SqlLiteBridge(libraryPath))
                .SingleInstance();


        }

        public static void Initialize()
        {
            IOC.Instance.Setup(new[] { typeof(ServicesModule), typeof(ViewModelModule), typeof(Bootstrapper), typeof(SQlCovidModule) });
        }
    }
}