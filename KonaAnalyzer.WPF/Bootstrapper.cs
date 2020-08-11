using System;
using System.IO;
using Autofac;
using KonaAnalyzer;
using KonaAnalyzer.Setup;
using KonaAnalyzer.SqlData;
using PolyhydraGames.SQLite;
using PolyhydraGames.SQLite.Interfaces; 

namespace Covid.WPF
{
    public class Bootstrapper : Module
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            var libraryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "PolyhydraGames"); // Library folder
            try
            {
                Directory.CreateDirectory(libraryPath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to create path at {libraryPath} or maybe {ex.Message}");
            }
            builder.Register<ISQLiteFactory>((ctx) => new SqlLiteBridge(libraryPath)).SingleInstance();
        }

        public static void Initialize()
        {
            IOC.Instance.Setup(new[] { typeof(ServicesModule), typeof(ViewModelModule), typeof(Bootstrapper),typeof(SQlCovidModule) });
        }
    }
}