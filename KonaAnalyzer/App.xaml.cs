using System;
using KonaAnalyzer.Data;
using Xamarin.Forms; 
using KonaAnalyzer.Views;

namespace KonaAnalyzer
{
    public partial class App : Application
    { 

        public App()
        {
            InitializeComponent(); 
            DependencyService.Register<InMemoryCovidSource>();
                DependencyService.Get<ICovidSource>().Load();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
