using System.ComponentModel;
using KonaAnalyzer.Setup;
using Xamarin.Forms;

namespace KonaAnalyzer.Views.Overview
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class OverviewPage : ContentPage
    {
        public OverviewViewModel ViewModel => (OverviewViewModel) BindingContext;
        public OverviewPage()
        {
            BindingContext = IOC.Get<OverviewViewModel>();
            InitializeComponent();
            
        }
    }
}