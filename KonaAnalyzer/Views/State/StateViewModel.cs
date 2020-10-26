using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using KonaAnalyzer.Data;
using PolyhydraGames.Extensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace KonaAnalyzer.ViewModels
{
    public class StateViewModel : StateControlViewModel
    {
        [Reactive] public bool LoadingCounties { get; set; } 
        [Reactive] public List<string> Counties { get; set; } 
        [Reactive] public DateTime MaxDate { get; set; } = DateTime.MaxValue;  
        public ICommand DateUpCommand { get; }
        public ICommand DateDownCommand { get; }
        public ICommand SortCommand { get; }
        [Reactive] public Sort Sort { get; set; }
        private readonly SourceList<StateControlViewModel> _sourceViewModels;
        private readonly ReadOnlyObservableCollection<StateControlViewModel> _countyViewModels;
        public IEnumerable<StateControlViewModel> CountyViewModels => _countyViewModels;
        public StateViewModel()
        {
            Title = "NA";
            _sourceViewModels = new SourceList<StateControlViewModel>();
            var sortChanged = this.WhenAnyValue(x => x.Sort).Select(x => x.GetSorter());
            _sourceViewModels.Connect()
                .Sort(sortChanged)
                .Bind(out _countyViewModels)
                .ObserveOn(RxApp.MainThreadScheduler)
                .DisposeMany()
                .Subscribe();
            DateUpCommand = ReactiveCommand.Create(() =>
            {
                if (Date != MaxDate) Date += TimeSpan.FromDays(1);
            });

            SortCommand = ReactiveCommand.Create<string, Unit>((x) =>
            {
                Sort = x.GetSort(Sort);
                return Unit.Default;
            });
            DateDownCommand = ReactiveCommand.Create(() => Date -= TimeSpan.FromDays(1));
            this.WhenAnyValue(x => x.Date).Where(x => x != null).Subscribe(x =>
               {
                   if (LoadingCounties || CountyViewModels == null) return;
                   foreach (var item in CountyViewModels)
                   {
                       item.Date = x;

                   }
               });
        
        
        }

        public override void OnStateUpdatedAsync(string state)
        {
            
            MaxDate =  DataStore.Latest;
            Date = MaxDate;
            base.OnStateUpdatedAsync(state);
            if (State == "All")
            {
                PopulateStates();
                PopulateSubStateViewModels();
            }
            else
            {
                PopulateCounties(state);
                PopulateCountyViewModels();
                County = "All";
            } 
        }
        public void Load(string state)
        {
            State = state;
            Title = state;
        }
        private void PopulateCounties(string state)
        {
            Counties = LocationStore.Counties(state).ToList();  
        }

        private void PopulateCountyViewModels()
        {
            LoadingCounties = true;
            _sourceViewModels.Clear();
            foreach (var item in Counties)
            {
                if (string.IsNullOrEmpty(item) || item == "All") continue;
                var vm = new StateControlViewModel();
                _sourceViewModels.Add(vm);
                Task.Run(()=>vm.Load(item, State, Date));
                
            }

            LoadingCounties = false;
        }

        private void PopulateStates()
        {
            var counties = LocationStore.States().ToList();
            Counties = counties;
        }

        private void PopulateSubStateViewModels()
        {
            LoadingCounties = true;
            _sourceViewModels.Clear();
            foreach (var item in Counties)
            {
                if (string.IsNullOrEmpty(item) || item == "All") continue;
                var vm = new StateControlViewModel();
                _sourceViewModels.Add(vm);
                 Task.Run(()=>vm.Load("All", item, Date));
                
            }

            LoadingCounties = false;
        }


    }

    public static class SortHelpers
    {
        public static Sort GetSort(this string command, Sort existingSort)
        {
            var existingSortString = existingSort.ToString();

            if (existingSortString.Contains(command))
            {
                return (command + (existingSortString.Contains("Ascending") ? "Descending" : "Ascending"))
                    .ToEnum<Sort>();
            }
            else
            {
                command = command + "Descending";
                var sort = command.ToEnum<Sort>();
                return sort;
            }
        }

        public static SortExpressionComparer<StateControlViewModel> GetSorter(this Sort x)
        {
            switch (x)
            {
                case Sort.DeadAscending: return SortExpressionComparer<StateControlViewModel>.Ascending(vm => vm.Dead);
                case Sort.DeadDescending: return SortExpressionComparer<StateControlViewModel>.Descending(vm => vm.Dead);
                case Sort.TotalAscending: return SortExpressionComparer<StateControlViewModel>.Ascending(vm => vm.Current);
                case Sort.TotalDescending: return SortExpressionComparer<StateControlViewModel>.Descending(vm => vm.Current);
                case Sort.AlphabeticalAscending: return SortExpressionComparer<StateControlViewModel>.Ascending(vm => vm.County);
                case Sort.AlphabeticalDescending: return SortExpressionComparer<StateControlViewModel>.Descending(vm => vm.County);
                case Sort.MortalityAscending: return SortExpressionComparer<StateControlViewModel>.Ascending(vm => vm.MortalityRate);
                case Sort.MortalityDescending: return SortExpressionComparer<StateControlViewModel>.Descending(vm => vm.MortalityRate);

                case Sort.RiskAscending: return SortExpressionComparer<StateControlViewModel>.Ascending(vm => vm.CurrentRiskRate);
                case Sort.RiskDescending: return SortExpressionComparer<StateControlViewModel>.Descending(vm => vm.CurrentRiskRate);

                case Sort.ChangeAscending: return SortExpressionComparer<StateControlViewModel>.Ascending(vm => vm.CurrentChange);
                case Sort.ChangeDescending: return SortExpressionComparer<StateControlViewModel>.Descending(vm => vm.CurrentChange);

                case Sort.PercentAscending: return SortExpressionComparer<StateControlViewModel>.Ascending(vm => vm.CurrentChangeRate);
                case Sort.PercentDescending: return SortExpressionComparer<StateControlViewModel>.Descending(vm => vm.CurrentChangeRate);

                case Sort.PopulationAscending: return SortExpressionComparer<StateControlViewModel>.Ascending(vm => vm.Population);
                case Sort.PopulationDescending: return SortExpressionComparer<StateControlViewModel>.Descending(vm => vm.Population);


                default: throw new ArgumentOutOfRangeException(nameof(x), x, null);
            }
        }

        //public static string ToFriendlyString(this Sort x)
        //{
        //    switch (x)
        //    {
        //        case Sort.DeadAscending: return "Deaths Asc.";
        //        case Sort.DeadDescending: return "Deaths Desc.";
        //        case Sort.TotalAscending: return "Total Asc.";
        //        case Sort.TotalDescending: return "Total Desc.";
        //        case Sort.AlphabeticalAscending: return "Alpha Asc.";
        //        case Sort.AlphabeticalDescending: return "Alpha Desc.";
        //        case Sort.MortalityAscending: return "Mort Asc.";
        //        case Sort.MortalityDeccending: return "Mort Desc.";
        //        default:
        //            throw new ArgumentOutOfRangeException(nameof(x), x, null);
        //    }
        //}
    }
}