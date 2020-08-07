using System;
using System.Reactive;
using System.Threading.Tasks;
using KonaAnalyzer.Data;
using ReactiveUI;

namespace KonaAnalyzer.Core.ViewModels
{
    public class LoadingViewModel : BaseViewModel
    {
        public LoadingViewModel(ICovidSource covid, IPopulationSource population) : base(covid, population)
        {
            Title = "Loading ...";

            LoadCommand = ReactiveCommand.CreateFromTask(Load);
            LoadCommand.IsExecuting.Subscribe(x => IsBusy = x);
        }

        private ReactiveCommand<Unit, Unit> LoadCommand;

        public async Task Load()
        {
            await Task.Run(() => DataStore.Load());
            await Task.Run(() => PopulationDataStore.Load());
            Title = "Loading ... Done";
        }

    }
}