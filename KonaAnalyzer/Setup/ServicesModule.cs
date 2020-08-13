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

    
        protected override void Load(ContainerBuilder builder)
        { 
            var assembly = typeof(ServicesModule).Assembly;
            var services = assembly.CreatableTypes().EndingWith("Service").ToArray();
            builder.RegisterTypes(services).AsImplementedInterfaces().AsSelf().SingleInstance();

            builder.RegisterType<InMemoryPopulationSource>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<LocationSource>().AsImplementedInterfaces().SingleInstance();
        }
    }
}

