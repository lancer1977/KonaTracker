using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core.Registration;
using KonaAnalyzer.Data;
using PolyhydraGames.Extensions;

namespace KonaAnalyzer
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
            var assembly = typeof(KonaAnalyzer.ServicesModule).Assembly;
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
            builder.RegisterType<InMemoryLiteCovidSource>().AsImplementedInterfaces().AsSelf().SingleInstance();
        }
    }
}

