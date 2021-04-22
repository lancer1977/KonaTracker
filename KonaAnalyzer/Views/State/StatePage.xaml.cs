using System.ComponentModel;
using KonaAnalyzer.Setup;
using KonaAnalyzer.ViewModels;
using KonaAnalyzer.Views.Overview;
using Xamarin.Forms;

namespace KonaAnalyzer.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class StatePage : ContentPage
    {

        public StateViewModel ViewModel => (StateViewModel)BindingContext;
        public StatePage()
        {
            BindingContext = IOC.Get<StateViewModel>();
            InitializeComponent();
            if (Device.RuntimePlatform == Device.UWP)
            {
                ToolbarItems.Add(new ToolbarItem()
                {
                    Text = " + ",
                    Command = ViewModel.DateUpCommand
                });

                ToolbarItems.Add(new ToolbarItem()
                {
                    Text = " - ",
                    Command = ViewModel.DateDownCommand
                });
                //< ToolbarItem Text = " + "  Command = "{Binding DateUpCommand}" />
   
                  //  < ToolbarItem   Text = " - "  Command = "{Binding DateDownCommand}" />
            }
        }

        void ListView_ItemSelected(System.Object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;
            var vm = e.SelectedItem as StateControlViewModel;
            (sender as ListView).SelectedItem = null;
   
           
            if (ViewModel.State != "All")
            {
                vm.CountySelected();

            }
            else
            { 
                vm.ItemSelected(); 
            }

        }
    }
}