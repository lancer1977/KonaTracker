
using KonaAnalyzer.Setup;
using KonaAnalyzer.ViewModels; 
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KonaAnalyzer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingPage : ContentPage
    {

        public LoadingPage()
        {
            InitializeComponent();
            BindingContext = IOC.Get<LoadingViewModel>();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing(); 
            (BindingContext as LoadingViewModel).OnAppearing();
        }


    }
}