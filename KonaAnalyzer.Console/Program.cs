using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using KonaAnalyzer.Console.Setup;
using KonaAnalyzer.Dapper;

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

   
 


    }
}
