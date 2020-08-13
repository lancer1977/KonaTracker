using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using KonaAnalyzer.Services;
using PolyhydraGames.Extensions;

namespace KonaAnalyzer.Setup
{
    public class ServicesModule : Autofac.Module
    {
        private IEnumerable<Type> _except;

        public   ServicesModule()
        {
            _except = new[] {typeof(InMemoryLiteCovidSource)};
        }
        protected override void Load(ContainerBuilder builder)
        { 
            var assembly = typeof(ServicesModule).Assembly;
            var services = assembly.CreatableTypes().EndingWith("Service").ToArray();
            builder.RegisterTypes(services).AsImplementedInterfaces().AsSelf().SingleInstance();

            var sources = assembly.CreatableTypes().EndingWith("Source").Except(_except).ToArray();
            builder.RegisterTypes(sources).AsImplementedInterfaces().AsSelf().SingleInstance();
        }
    }

    public class InMemoryCovidModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        { 
            builder.RegisterType<InMemoryCovidSource>().AsImplementedInterfaces().AsSelf().SingleInstance();
        }
    }
}

