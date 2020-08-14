using Autofac;
using System;
using System.IO;
using Autofac;
using KonaAnalyzer.Setup;
using PolyhydraGames.SQLite;
using PolyhydraGames.SQLite.Interfaces;

namespace KonaAnalyzer.Mac
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
            IOC.Instance.Setup(new[] { typeof(ServicesModule), typeof(ViewModelModule), typeof(Bootstrapper), typeof(InMemoryCovidModule) });
        }
    }

}


