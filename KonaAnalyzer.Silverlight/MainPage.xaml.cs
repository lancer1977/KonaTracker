using System;
using System.Windows.Controls;
using Autofac;
using KonaAnalyzer.Core.ViewModels;
using KonaAnalyzer.Data;
using KonaAnalyzer.Services;

namespace KonaAnalyzer.Silverlight
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            IOC.Container.Resolve<ICovidSource>().Load();
            IOC.Container.Resolve<IPopulationSource>().Load();
            DataContext = IOC.Container.Resolve<StateViewModel>();
            // Enter construction logic here...
        }

        public void Button_Click(object sender, EventArgs e)
        {
            ((StateViewModel)DataContext).Title = "I am changed text!";
        }

        public StateViewModel ViewModel => (StateViewModel)DataContext;
 
        //void ListView_ItemSelected(System.Object sender, SelectionChangedEventArgs e)
        //{
        //    if (e.SelectedItem == null || ViewModel.State != "All") return;
        //    var vm = e.SelectedItem as StateControlViewModel;
        //    vm.ItemSelected();
        //    (sender as ListView).SelectedItem = null;
        //}
    }


 
}
