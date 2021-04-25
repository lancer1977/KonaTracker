
using System;
using System.IO;
using Autofac;
using KonaAnalyzer.Dapper;
using PolyhydraGames.SQLite;
using PolyhydraGames.SQLite.Interfaces;

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
            builder.Register(x => new KonaContextService("Server=192.168.0.168;Database=Kona;MultipleActiveResultSets=true;User Id=sa;Password=biz$314$!35##!21;"));
            builder.RegisterType<DapperCovidSource>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<DapperLocationSource>().AsImplementedInterfaces().SingleInstance();
        }
    }

}



 