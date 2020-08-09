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
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            //(BindingContext as LoadingViewModel).LoadCommand.Execute(Unit.Default);
            (BindingContext as LoadingViewModel).OnAppearing();
        }


    }
}