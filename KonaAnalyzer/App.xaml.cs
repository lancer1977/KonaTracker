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
            IOC.Instance.Setup(new[] { typeof(ServicesModule), typeof(ViewModelModule) });
            InitializeComponent();
#if !DEBUG
            AppCenter.Start(Configs.AppCenterSecret, typeof(Analytics), typeof(Crashes), typeof(Microsoft.AppCenter.Distribute.Distribute)); 
#endif
       
            HttpService.Instance = new HttpService();
            MainPage = new MainPage(); ;
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
