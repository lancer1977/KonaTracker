using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Autofac;
using DynamicData;
using KonaAnalyzer.Data;
using KonaAnalyzer.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace KonaAnalyzer.Core.ViewModels
{
    public class StateViewModel : StateControlViewModel
    {
        private readonly IComponentContext _context;
        [Reactive] public bool LoadingCounties { get; set; }

        //[Reactive] public ChangeModel Model { get; set; }
        [Reactive] public List<string> Counties { get; set; }

        [Reactive] public List<DateTime> Dates { get; set; } = new List<DateTime>();

        [Reactive] public DateTime MaxDate { get; set; } = DateTime.MaxValue;
        [Reactive] public DateTime MinDate { get; set; } = DateTime.MinValue;

        public ICommand DateUpCommand { get; }
        public ICommand DateDownCommand { get; }
        public ICommand SortCommand { get; }
        [Reactive] public Sort Sort { get; set; }
        private readonly SourceList<StateControlViewModel> _sourceViewModels;
        private readonly ReadOnlyObservableCollection<StateControlViewModel> _countyViewModels;
        public IEnumerable<StateControlViewModel> CountyViewModels => _countyViewModels;
        public StateViewModel(ICovidSource covid, IPopulationSource population, INavigator navigation, IComponentContext context) : base(covid, population, navigation)
        {
            _context = context;
            Title = "NA";
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
                   if (LoadingCounties) return;
                   foreach (var item in CountyViewModels)
                   {
                       item.Date = x;

                   }
               });
            var sortChanged = this.WhenAnyValue(x => x.Sort).Select(x => x.GetSorter());
            //this.WhenAnyValue(x => x.Sort).Select(x => x.ToFriendlyString()).ToProperty(this, x => x.SortText, out _sortText);
            _sourceViewModels = new SourceList<StateControlViewModel>();
            var myOperation = _sourceViewModels.Connect()
                //.Filter(trade => trade.Status == TradeStatus.Live)
                //.Transform(trade => new TradeProxy(trade))
                .Sort(sortChanged)
                //.ObserveOnDispatcher()
                .Bind(out _countyViewModels)
                .DisposeMany()
                .Subscribe();
        }

        public override async Task OnStateUpdatedAsync(string state)
        {
            Date = DataStore.LastDate(state);
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

            var dateRange = new List<DateTime>();
            for (var y = 0; y < 30; y++)
            {
                dateRange.Add(Date.Value - TimeSpan.FromDays(y));
            }

            Dates = dateRange;
            MinDate = dateRange.Last();
            MaxDate = dateRange.First();


            await base.OnStateUpdatedAsync(state);
        }
        public void Load(string state)
        {
            State = state;
            Title = state;
        }
        private void PopulateCounties(string state)
        {
            var counties = DataStore.Counties(state);
            //counties.Insert(0, "All");
            Counties = counties;
        }

        private async void PopulateCountyViewModels()
        {
            LoadingCounties = true;
            _sourceViewModels.Clear();
            foreach (var item in Counties)
            {
                if (string.IsNullOrEmpty(item) || item == "All") continue;
                var vm = _context.Resolve<StateControlViewModel>();

                await Task.Run(() =>
                {
                    vm.Load(item, State, Date);
                });
                _sourceViewModels.Add(vm);
            }

            LoadingCounties = false;
        }

        private void PopulateStates()
        {
            var counties = DataStore.States;
            Counties = counties;
        }

        private async void PopulateSubStateViewModels()
        {
            LoadingCounties = true;
            _sourceViewModels.Clear();
            foreach (var item in Counties)
            {
                if (string.IsNullOrEmpty(item) || item == "All") continue;
                var vm = _context.Resolve<StateControlViewModel>();
                await Task.Run(() => { vm.Load("All", item, Date); });
                _sourceViewModels.Add(vm);
            }

            LoadingCounties = false;
        }


    }

}