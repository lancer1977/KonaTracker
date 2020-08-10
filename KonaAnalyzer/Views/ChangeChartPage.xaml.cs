using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using KonaAnalyzer.ViewModels;
using ReactiveUI;
using Xamarin.Forms;

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
            this.InitializeComponent();

            BindingContext = IOC.Get<ChangeChartViewModel>(); 
        }
    }
}
