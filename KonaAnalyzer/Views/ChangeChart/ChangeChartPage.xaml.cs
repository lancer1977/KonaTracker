using KonaAnalyzer.Setup;
using KonaAnalyzer.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace KonaAnalyzer.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChangeChartPage
    {
        public ChangeChartViewModel ViewModel => (ChangeChartViewModel)BindingContext;
        public ChangeChartPage()
        {    
            BindingContext = IOC.Get<ChangeChartViewModel>(); 
            this.InitializeComponent();

        
        }
    }
}
