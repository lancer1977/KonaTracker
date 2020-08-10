using System;
using System.Diagnostics;

namespace KonaAnalyzer.Console
{
    class MainClass
    {
        public static   void Main(string[] args)
        {
            Debug.WriteLine("Hello World!");
            var tools = new CovidTools();
              tools.StartPump();
        }
    }
}
