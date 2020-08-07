using System;
using System.ComponentModel;
using System.Windows.Controls;
using Autofac;
using KonaAnalyzer.Core.ViewModels;
using KonaAnalyzer.Services;

namespace KonaAnalyzer.Silverlight
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class StatePage : Page
    {

        public StateViewModel ViewModel => (StateViewModel) DataContext;
        public StatePage()
        {
            InitializeComponent();
            DataContext = IOC.Container.Resolve<StateViewModel>();
 
        }

        void ListView_ItemSelected(System.Object sender,EventArgs e)
        {
            if (e.SelectedItem == null || ViewModel.State != "All") return;
           var vm =  e.SelectedItem as StateControlViewModel;
            vm.ItemSelected();
            (sender as ListView).SelectedItem = null;
        }
    }
}