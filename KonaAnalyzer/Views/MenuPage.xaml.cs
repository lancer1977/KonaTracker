using KonaAnalyzer.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using DynamicData;
using KonaAnalyzer.Data;
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
        private ICovidSource Source => InMemoryCovidSource.Instance;
        ObservableCollection<HomeMenuItem> menuItems = new ObservableCollection<HomeMenuItem>();

        public MenuPage()
        {
            InitializeComponent();


            menuItems.AddRange(new[]{
                new HomeMenuItem() {Title = "Overview"},
            new HomeMenuItem() {Title = "Reload Data"},
            new HomeMenuItem() {Title = "About"},
            new HomeMenuItem() {Title = "------------------"},
            new HomeMenuItem() {Title = "All"}
            });
            Source.WhenAnyValue(x => x.Loaded).Subscribe(loaded =>
            {
                if (loaded == false) return;
                menuItems.AddRange(Source.States
                    .Select(x => new HomeMenuItem() { Title = x }));
            });
            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                // var id = (int)((HomeMenuItem)e.SelectedItem).Id;
                await RootPage.NavigateFromMenu(e.SelectedItem.ToString());
            };
        }


    }
}