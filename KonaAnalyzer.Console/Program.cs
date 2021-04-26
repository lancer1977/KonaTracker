using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;  

namespace KonaAnalyzer.Cli
{
    class Program
    {
        public static   async Task Main(string[] args)
        {
            Debug.WriteLine("Hello World!"); 
            var tools = new CovidTools();
             await tools.TestSources();
        }

   
 


    }
}
