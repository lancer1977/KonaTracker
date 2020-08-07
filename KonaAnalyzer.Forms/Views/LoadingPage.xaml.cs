using Autofac;
using KonaAnalyzer.Core.ViewModels; 
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
            BindingContext = IOC.Container.Resolve<LoadingViewModel>();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await (BindingContext as LoadingViewModel).Load();
            await  (Application.Current.MainPage as MainPage).NavigateFromMenu("All");
        }
    }
}