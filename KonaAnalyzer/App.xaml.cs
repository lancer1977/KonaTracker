using System;
using KonaAnalyzer.Data;
using KonaAnalyzer.Services;
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
            HttpService.Instance = new HttpService();
            MainPage = new MainPage(); ;
        }

        protected override async void OnStart()
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
