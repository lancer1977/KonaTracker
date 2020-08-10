using System;
using KonaAnalyzer.Data;
using KonaAnalyzer.Services;
using Xamarin.Forms;
using KonaAnalyzer.Views; 

namespace KonaAnalyzer
{
    public partial class App : Application
    {

        public App( )
        {
  
            InitializeComponent();
#if !DEBUG
            AppCenter.Start(Configs.AppCenterSecret, typeof(Analytics), typeof(Crashes), typeof(Microsoft.AppCenter.Distribute.Distribute)); 
#endif
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(Configs.SyncfusionKey);
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
