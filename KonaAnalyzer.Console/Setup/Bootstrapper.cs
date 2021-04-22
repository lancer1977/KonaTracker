using Autofac;
using KonaAnalyzer.Setup;
using KonaAnalyzer.SqlData;

namespace KonaAnalyzer.Console.Setup
{
    public class Bootstrapper : Module
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>


        public static void Initialize()
        {
            IOC.Instance.Setup(new[] { typeof(ServicesModule), typeof(ViewModelModule), typeof(Bootstrapper),typeof(SQlModule),typeof(SQLiteBootstrapper) });
        }
    }
}