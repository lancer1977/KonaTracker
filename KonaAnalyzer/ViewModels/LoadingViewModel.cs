using System.Reactive;
using System.Threading.Tasks;
using KonaAnalyzer.Data;
using ReactiveUI;
using Xamarin.Forms;
using System;

namespace KonaAnalyzer.ViewModels
{
    public class LoadingViewModel : BaseViewModel
    {
        public LoadingViewModel()
        {
            Title = "Loading ..."; 

            LoadCommand = ReactiveCommand.CreateFromTask(Load);
            LoadCommand.IsExecuting.Subscribe(x => IsBusy = x);
        }

        private ReactiveCommand<Unit, Unit> LoadCommand;

        public async Task Load()
        {
            await Task.Run(() => DependencyService.Get<ICovidSource>().Load());
            Title = "Loading ... Done";
        }

    }
}