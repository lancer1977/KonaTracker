using System.Linq;
using Autofac;
using Autofac.Core.Registration;
using PolyhydraGames.Extensions;

namespace KonaAnalyzer
{
    public class ServicesModule : Autofac.Module
    { 
        protected override void Load(ContainerBuilder builder)
        { 
            var assembly = typeof(KonaAnalyzer.ServicesModule).Assembly;
            var services = assembly.CreatableTypes().EndingWith("Service").ToArray();
            builder.RegisterTypes(services).AsImplementedInterfaces().AsSelf().SingleInstance();

            var sources = assembly.CreatableTypes().EndingWith("Source").ToArray();
            builder.RegisterTypes(sources).AsImplementedInterfaces().AsSelf().SingleInstance();
        }
    }
}

