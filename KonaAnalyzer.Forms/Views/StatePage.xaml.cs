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
    public partial class StatePage : ContentPage
    {

        public StateViewModel ViewModel => (StateViewModel) BindingContext;
        public StatePage()
        {
            InitializeComponent();
            BindingContext = IOC.Container.Resolve<StateViewModel>();
 
        }

        void ListView_ItemSelected(System.Object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null || ViewModel.State != "All") return;
           var vm =  e.SelectedItem as StateControlViewModel;
            vm.ItemSelected();
            (sender as ListView).SelectedItem = null;
        }
    }
}