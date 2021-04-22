using KonaAnalyzer.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using KonaAnalyzer.Data.Interface;
using KonaAnalyzer.Setup;
using ReactiveUI;
using Xamarin.Forms;

namespace KonaAnalyzer.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        private ICovidSource Source => IOC.Get<ICovidSource>();// InMemoryCovidSource.Instance;
        ObservableCollection<HomeMenuItem> menuItems = new ObservableCollection<HomeMenuItem>();

        public MenuPage()
        {
            InitializeComponent();


            menuItems.AddRange(new[]{

            new HomeMenuItem() {Title = "Reload Data"},
            new HomeMenuItem() {Title = "About"},
            new HomeMenuItem() {Title = "------------------"},

            });
            Source.WhenAnyValue(x => x.LoadState).Where(x => x == LoadedState.Loaded).ObserveOn(RxApp.MainThreadScheduler).Subscribe(state =>
            {
                menuItems.Add(new HomeMenuItem() { Title = "Overview" });
                menuItems.Add(new HomeMenuItem() { Title = "Change Charts" });
                menuItems.Add(new HomeMenuItem() { Title = "All" });
                menuItems.AddRange(IOC.Get<ILocationSource>().States().Select(x => new HomeMenuItem() { Title = x }));
            });
            //Source.PropertyChanged += (sender, args) =>
            //{
            //    if (args.PropertyName == "LoadState" && Source.LoadState == LoadedState.Loaded)
            //    {
            //        menuItems.Add(new HomeMenuItem() { Title = "Overview" });
            //        menuItems.Add(new HomeMenuItem() { Title = "Change Charts" });
            //        menuItems.Add(new HomeMenuItem() { Title = "All" });
            //        menuItems.AddRange(Source.States.Select(x => new HomeMenuItem() { Title = x }));
            //    }


            //};
            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                // var id = (int)((HomeMenuItem)e.SelectedItem).Id;
                RootPage.NavigateFromMenu(e.SelectedItem.ToString());
            };
        }


    }
}