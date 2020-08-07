using System;
using System.ComponentModel;
using Autofac;
using KonaAnalyzer.Core.ViewModels; 
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KonaAnalyzer.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class OverviewPage : ContentPage
    {
        public OverviewViewModel ViewModel => (OverviewViewModel) BindingContext;
        public OverviewPage()
        {
            InitializeComponent();
            BindingContext = IOC.Container.Resolve<OverviewViewModel>();
        }
    }
}