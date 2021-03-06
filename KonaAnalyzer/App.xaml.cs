﻿using System.Diagnostics;
using Xamarin.Forms;
using KonaAnalyzer.Views;
using PolyhydraGames.Core.Data;

#if !DEBUG
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;

#endif

namespace KonaAnalyzer
{
    public partial class KonaApp : Application
    {

        public KonaApp( )
        {
  
            InitializeComponent();
#if !DEBUG
            AppCenter.Start(Configs.AppCenterSecret,  typeof(Crashes), typeof(Distribute));  
#endif
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(Configs.SyncfusionKey);
         
            MainPage = new MainPage(); ;
            foreach(var item in Application.Current.Resources.Keys)
                Debug.WriteLine(item);
            
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
