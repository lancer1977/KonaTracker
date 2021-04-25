using System.Data;
using System.Data.SqlClient;
using PolyhydraGames.Core.Interfaces;

namespace KonaAnalyzer.Cli.Setup
{
    public class TSQLDbContextService : IDBConnectionFactory
    {
        private static string _dbString = "Server=192.168.0.168;Database=Kona;MultipleActiveResultSets=true;User Id=sa;Password=biz$314$!35##!21;";

        //private static string _dbString = "Server=tcp:polyhydra-games-db.database.windows.net,1433;Initial Catalog=PolyhydraDB;Persist Security Info=False;User ID=polydblogin;Password=biz$314$!35##!21;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public bool IsSQLite => false;

        public IDbConnection GetConnection()
        {
            return new SqlConnection(_dbString);
        }
 


    }
}