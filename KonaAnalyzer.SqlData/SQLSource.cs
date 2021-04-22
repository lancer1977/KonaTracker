using System.Threading.Tasks;
using KonaAnalyzer.Data.Model;
using KonaAnalyzer.Services;
using PolyhydraGames.SQLite.Interfaces;
using SQLite;

namespace KonaAnalyzer.SqlData
{
    public abstract class SQLSource<T> : BaseSource where T : new()
    {
        private readonly ISQLiteFactory _factory;


        protected SQLSource(ISQLiteFactory factory)
        {
            _factory = factory;
        }
        private SQLiteConnection _connection;

        protected SQLiteConnection Connection
        {
            get
            {
                if (_connection != null) return _connection;
                _connection = _factory.CreateConnection();
                _connection.CreateTable<T>();
                return _connection;
            }
        }

        public TableQuery<T> Table => Connection.Table<T>();
    }
}