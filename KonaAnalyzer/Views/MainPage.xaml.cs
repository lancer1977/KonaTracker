using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using KonaAnalyzer.Models;

namespace KonaAnalyzer.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {
        StatePage _statePage = new StatePage();
        OverviewPage _overviewPage = new OverviewPage();
        public MainPage()
        {
            InitializeComponent();

            MasterBehavior = MasterBehavior.Popover; 
            Detail = new NavigationPage(_statePage);
        }

        public async Task NavigateFromMenu(string id)
        {
            //if (Device.RuntimePlatform == Device.Android)
            //    await Task.Delay(100);
            if (id == "Overview")
            {
                Detail = new NavigationPage(_overviewPage);
            }
            else
            {
                Detail = new NavigationPage(_statePage);
                _statePage.ViewModel.LoadState(id);
            }

      
            if(Device.RuntimePlatform != Device.WPF)
                IsPresented = false;
        }
    }
}