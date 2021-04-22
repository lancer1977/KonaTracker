using System.Data;
using System.Data.SqlClient;
using Dapper;
using PolyhydraGames.Core.Interfaces;

namespace KonaAnalyzer.Dapper
{
    public class KonaContextService : IDBConnectionFactory
    {
        public KonaContextService(string connection)
        {
            _dbString = connection;
        }

        private readonly string _dbString;

        //private static string _dbString = "Server=tcp:polyhydra-games-db.database.windows.net,1433;Initial Catalog=PolyhydraDB;Persist Security Info=False;User ID=polydblogin;Password=biz$314$!35##!21;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public bool IsSQLite => false;

        public IDbConnection GetConnection()
        {
            return new SqlConnection(_dbString);
        }
         
 

    }
}