﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using KonaAnalyzer.Data;
using Xamarin.Forms;

namespace KonaAnalyzer.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {
        StatePage _statePage;
        StatePage StatePage => _statePage ?? (_statePage = new StatePage());
        OverviewPage _overviewPage;
        OverviewPage OverviewPage => _overviewPage ?? (_overviewPage = new OverviewPage());
        public MainPage()
        {
            InitializeComponent();

            MasterBehavior = MasterBehavior.Popover;


            //  Detail = new NavigationPage(new LoadingPage());

        }

        public void NavigateFromMenu(string id)
        {
            //if (Device.RuntimePlatform == Device.Android)
            //    await Task.Delay(100);
            switch (id)
            {
                case "Overview":
                    Detail = new NavigationPage(OverviewPage);
                    break;
                case "Reload Data":
                    Detail = new NavigationPage(new LoadingPage());
                    //DependencyService.Get<ICovidSource>().Load();
                    break;
                case "Change Charts":
                    Detail = new NavigationPage(new ChangeChartPage());
                    break;
                case "About":
                    Detail = new NavigationPage(new AboutPage());
                    break;
                default:
                    if (id.Contains("-----")) return;
                    Detail = new NavigationPage(StatePage);
                    _statePage.ViewModel.Load(id);
                    break;
            }


            if (Device.RuntimePlatform != Device.WPF)
                IsPresented = false;
        }


    }
}