using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using KonaAnalyzer.Console.Setup;
using PolyhydraGames.SQLite;
using PolyhydraGames.SQLite.Interfaces;

namespace KonaAnalyzer.Cli
{
    class Program
    {
        public static   async Task Main(string[] args)
        {
            Debug.WriteLine("Hello World!");
            Bootstrapper.Initialize();
            var tools = new CovidTools();
             await tools.TestSources();
        }

        public static async Task Kona()
        {
            var ctx = new KonaContextService(_dbString);
            var locations = new DapperLocationSource(ctx);
            //await locations.LoadAsync();
            var source = new DapperCovidSource(ctx, locations);
            await source.LoadAsync();
             
            //await pop.LoadAsync();
        }
        private static void InitializeDBs()
        {
            var sqlLite = new TSQLDbContextService();
            sqlLite.Initialize();
             

        } 


    }
}
