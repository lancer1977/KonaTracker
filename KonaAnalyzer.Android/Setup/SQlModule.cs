﻿using Autofac;
using KonaAnalyzer.Data.SQLite;

namespace KonaAnalyzer.Droid.Setup
{
    public class SQlModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SQLCovidSource>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<SQLLocationSource>().AsImplementedInterfaces().AsSelf().SingleInstance();
        }
    }
}