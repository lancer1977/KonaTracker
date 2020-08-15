using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using KonaAnalyzer.Data;
using KonaAnalyzer.ViewModels;
using ReactiveUI;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KonaAnalyzer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
     
        public AboutPage()
        {
            InitializeComponent();
        }
         
    }
}