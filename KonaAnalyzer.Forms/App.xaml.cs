﻿using Autofac;
using KonaAnalyzer.Core.ViewModels;
using KonaAnalyzer.Data;
using Xamarin.Forms;
using KonaAnalyzer.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms.Xaml;

namespace KonaAnalyzer
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
#if !DEBUG
            AppCenter.Start(Configs.AppCenterSecret, typeof(Analytics), typeof(Crashes), typeof(Microsoft.AppCenter.Distribute.Distribute)); 
#endif


            var builder = new ContainerBuilder();
            builder.RegisterType<Navigator>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<InMemoryCovidSource>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<InMemoryPopulationSource>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<StateViewModel>();
            builder.RegisterType<StateControlViewModel>();
            builder.RegisterType<OverviewViewModel>();
            builder.RegisterType<LoadingViewModel>();
            IOC.Container = builder.Build();

            MainPage = new MainPage(); ;
        }

        protected override void OnStart()
        {

        }

        protected override void OnSleep()
        {
        }

        protected override async void OnResume()
        {

        }

        public void GotoMainPage()
        {
            if (MainPage is MainPage)
            {

            }
            else
            {

                MainPage = new MainPage();
            }
        }
    }
}