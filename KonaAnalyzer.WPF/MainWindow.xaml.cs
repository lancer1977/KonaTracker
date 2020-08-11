
using Syncfusion.SfChart.XForms.WPF;

namespace Covid.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
    {
        public MainWindow()
        {
            InitializeComponent();
            Bootstrapper.Initialize();
            SfChartRenderer.Init();
            Xamarin.Forms.Forms.Init();
            LoadApplication(new KonaAnalyzer.App());
        }
    }
    }

