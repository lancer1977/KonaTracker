using System.Linq;
using Autofac;
using PolyhydraGames.Extensions;

namespace KonaAnalyzer
{
    public class ViewModelModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<DataLoader>().AsImplementedInterfaces().SingleInstance();
            //builder.RegisterType<Constants>().AsImplementedInterfaces().SingleInstance();
            //builder.RegisterType<ResourceDB>().AsSelf().SingleInstance();
            //builder.Register((x) => (App)Application.Current).As<IApp>().SingleInstance();
            var assembly = typeof(KonaAnalyzer.ServicesModule).Assembly;
            var types = assembly.CreatableTypes().EndingWith("ViewModel").ToArray();
            builder.RegisterTypes(types).AsImplementedInterfaces().AsSelf().SingleInstance();
        }
    }
}

