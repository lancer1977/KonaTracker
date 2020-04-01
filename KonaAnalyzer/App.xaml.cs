using System;
using KonaAnalyzer.Data;
using Xamarin.Forms;
using KonaAnalyzer.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace KonaAnalyzer
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            AppCenter.Start("9d772b2e-8b42-4cf9-9204-fbc30a462e71",
                typeof(Analytics), typeof(Crashes), typeof(Microsoft.AppCenter.Distribute.Distribute)); 
            DependencyService.Register<InMemoryCovidSource>();
            MainPage = new MainPage();;
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
