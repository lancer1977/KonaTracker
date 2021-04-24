using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Dapper;
using PolyhydraGames.Core.Interfaces;
using PolyhydraGames.Core.IOC;
using PolyhydraGames.Pathfinder.Data.Interface.Dto;

namespace KonaAnalyzer.Cli
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

        public void Initialize()
        {
            SetupTables();
            //var types = new[] { typeof(IArmorDto) };
            Populate<IMagicItemDto>();
            Populate<IAmmoDto>();
            Populate<IArmorDto>();
            Populate<IClassFeatureDto>();
            Populate<IClassDto>();
            Populate<IDeedDto>();
            Populate<IPowerDto>();
            Populate<ISpecializationDto>();


            Populate<ISpellDto>();
            Populate<IWeaponDto>();
            Populate<IWeaponFeatureDto>();
            Populate<IWordDto>();

            Populate<IConditionDto>();
            Populate<IFeatDto>();
            Populate<ILanguageDto>();

            Populate<IMonsterDto>();
            Populate<IMundaneItemDto>();


            Populate<IRaceDto>();
            Populate<IRacialTraitDto>();
            Populate<ISkillDto>();
            Populate<ITraitDto>();
        }

        private void Populate<T>()
        {
            Console.WriteLine("Injecting for " + typeof(T).Name);
            using var _context = GetConnection();
            var idb = IOC.Get<IInitializeDataSource<T>>();
            var db = IOC.Get<IDataSource<T>>();
            idb.Initialize(db);
            Console.WriteLine("Done in " + typeof(T).Name);
        }

        public bool ExecuteBool(string query)
        {
            using var connection = GetConnection() as SqlConnection;
            var exeuteScalar = (int)connection.ExecuteScalar(query);
            return exeuteScalar == 1;
        }

        private void SetupTables()
        {
            if (ExecuteBool(@" 
  IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'ArmorDto')
  )Select 1 else select 0")) return;
            using var connection = GetConnection() as SqlConnection;
            foreach (var file in Directory.GetFiles(".//Table//Create", "*.sql", SearchOption.AllDirectories))
            {
                Console.WriteLine(file);
                var test = File.ReadAllText(file);

                try
                {
                    Console.WriteLine(connection.Execute(test));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }



        }
    }
}