using System;
using System.Reflection;
using PolyhydraGames.Core.Interfaces;

namespace KonaAnalyzer.Cli.Setup
{
    public class StorageFolder : IStorageFolder
    {
        public string Get()
        {
            //var folder = Environment.CurrentDirectory;
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            Console.WriteLine(folder);
            return folder; // Documents folder
        }
    }

    public class Bootstrapper : PolyhydraGames.Core.Setup.IoCBootstrapper
    {
        public static void Run()
        {
            var item = new Bootstrapper();
            item.Setup();
        }
        private Bootstrapper()
        {
            this.Assemblies.Add(Assembly.Load("PolyhydraGames.Pathfinder.Data.Dapper"));
            this.Assemblies.Add(Assembly.Load("PolyhydraGames.Pathfinder.Data.Source.Restful"));
            Modules.Add(typeof(ConsoleModule));
        }
        protected override void ValidateRegistration()
        {
         
        }

        protected override void ViewFactoryRegistration()
        {
            
        }

        protected override void FollowupAction()
        {
            

        }
    }
}