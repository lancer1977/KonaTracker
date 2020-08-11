using System;
using Autofac;
using KonaAnalyzer.Setup;
using KonaAnalyzer.SqlData;
using PolyhydraGames.SQLite;
using PolyhydraGames.SQLite.Interfaces;

namespace KonaAnalyzer.Droid
{
    public class Bootstrapper : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.Register((ctx) => new SqlLiteBridge(Environment.GetFolderPath(Environment.SpecialFolder.Personal))).As<ISQLiteFactory>().SingleInstance();
        }

        public static void Initialize()
        {
            IOC.Instance.Setup(new[] { typeof(ServicesModule), typeof(ViewModelModule), typeof(Bootstrapper),typeof(SQlCovidModule) });
        }
    }
}