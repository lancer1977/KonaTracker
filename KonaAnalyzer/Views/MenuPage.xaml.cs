﻿using KonaAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using KonaAnalyzer.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KonaAnalyzer.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        List<HomeMenuItem> menuItems;
        public MenuPage()
        {
            InitializeComponent();
            menuItems = DependencyService.Get<ICovidSource>().States.Select(x=>new HomeMenuItem(){Title=x}).ToList();
            menuItems.Insert(0,new HomeMenuItem(){Title="Overview"});
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