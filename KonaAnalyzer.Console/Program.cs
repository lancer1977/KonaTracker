using System;
using System.Threading.Tasks;
using KonaAnalyzer.Cli.Setup;
using KonaAnalyzer.Dapper;

namespace KonaAnalyzer.Cli
{
    class Program
    {
        private static string _dbString = "Server=192.168.0.168;Database=Kona;MultipleActiveResultSets=true;User Id=sa;Password=biz$314$!35##!21;";

        // private static string _dbString = "Server=tcp:polyhydra-games-db.database.windows.net,1433;Initial Catalog=PolyhydraDB;Persist Security Info=False;User ID=polydblogin;Password=biz$314$!35##!21;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        static void Main(string[] args)
        {
            Bootstrapper.Run();
            Console.WriteLine("Hello World!");
            //GetLengths();

            Kona().GetAwaiter().GetResult();
            //InitializeDBs();
            Console.WriteLine("Done");
            Console.ReadKey();
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
