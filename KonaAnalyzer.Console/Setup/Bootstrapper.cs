using Autofac;
using KonaAnalyzer.Dapper;
using KonaAnalyzer.Setup; 
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
            IOC.Instance.Setup(new[] {
                typeof(SQLiteBootstrapper)  
             
            });
        }
    }
}