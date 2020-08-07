using System.Threading.Tasks;
using KonaAnalyzer.Services;
using KonaAnalyzer.Views;
using Xamarin.Forms;

namespace KonaAnalyzer
{
    public class Navigator : INavigator
    {
        public async Task Navigate(string state)
        {
            var nav = (Application.Current.MainPage as MasterDetailPage).Detail.Navigation;
            var statePage = new StatePage();
            statePage.ViewModel.Load(state);
            await nav.PushAsync(statePage);
        }
    }
}