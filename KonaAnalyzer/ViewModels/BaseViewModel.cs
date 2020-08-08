﻿using System.Runtime.CompilerServices;
using KonaAnalyzer.Data;
using Xamarin.Forms;
using ReactiveUI.Fody.Helpers;

namespace KonaAnalyzer.ViewModels
{
    public class BaseViewModel : ReactiveUI.ReactiveObject
    {
        public ICovidSource DataStore => InMemoryCovidSource.Instance;
        public IPopulationSource PopulationDataStore => InMemoryPopulationSource.Instance;
        [Reactive] public bool IsBusy { get; set; }
        [Reactive] public string Title { get; set; }


    }
}
