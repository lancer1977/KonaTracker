using System.Linq;
using System.Reflection;
using Autofac;
using PolyhydraGames.Extensions;
using Module = Autofac.Module;

namespace KonaAnalyzer.Cli.Setup
{
    public class ConsoleModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            //builder.RegisterModule<DeviceModule>();
            builder.RegisterModule<SQLiteBootstrapper>();
            builder.RegisterType<Cli.TSQLDbContextService>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<StorageFolder>().AsImplementedInterfaces().SingleInstance();
            var dapper = Assembly.Load("PolyhydraGames.Pathfinder.Data.Dapper");
            var rest = Assembly.Load("PolyhydraGames.Pathfinder.Data.Source.Restful");

            var types = rest.CreatableTypes().EndingWith("DataSource").ToList();
            types.AddRange(dapper.CreatableTypes().EndingWith("Database"));
            builder.RegisterTypes(types.ToArray()).AsImplementedInterfaces().AsSelf().SingleInstance();

            //builder.RegisterType<FakePremiums>().As<IBillingService>().SingleInstance();
            ////builder.RegisterType<AzureAuthenticatorService>().AsImplementedInterfaces().SingleInstance();
            //builder.RegisterType<SoundService>().As<ISoundService>().SingleInstance();
            //builder.RegisterType<BillingService>().AsImplementedInterfaces().SingleInstance();
            //builder.RegisterType<GenericPicker>().AsImplementedInterfaces().SingleInstance();
            //builder.RegisterType<PopupDialogService>().AsImplementedInterfaces().SingleInstance();
        }
    }
}