using System.Windows;
using Autofac;
using KonaAnalyzer.Core.ViewModels;
using KonaAnalyzer.Data;
using KonaAnalyzer.Services;

namespace KonaAnalyzer.Silverlight
{
    public sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();

            // Enter construction logic here...

            var builder = new ContainerBuilder();
            builder.RegisterType<Navigator>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<InMemoryCovidSource>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<InMemoryPopulationSource>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<StateViewModel>();
            builder.RegisterType<StateControlViewModel>();
            builder.RegisterType<OverviewViewModel>();
            builder.RegisterType<LoadingViewModel>();
            IOC.Container = builder.Build();
            var mainPage = new MainPage();
            Window.Current.Content = mainPage;
        }
    }
}
