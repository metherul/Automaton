using Autofac;
using Automaton.Model.Interfaces;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;
using Automaton.ViewModel.Utilities;
using GalaSoft.MvvmLight.Command;
using System.Threading;
using System.Threading.Tasks;

namespace Automaton.ViewModel
{
    public class NexusLoginViewModel : ViewModelBase, INexusLoginViewModel
    {
        private readonly IViewController _viewController;
        private readonly INexusSso _nexusSso;
        private readonly INexusApi _nexusApi;

        public AsyncCommand LoginToNexusCommand => new AsyncCommand(LoginToNexus);
        public RelayCommand ContinueOfflineCommand => new RelayCommand(ContinueOffline);

        public bool IsLoggingIn { get; set; }

        public NexusLoginViewModel(IComponentContext components)
        {
            _viewController = components.Resolve<IViewController>();

            _nexusSso = components.Resolve<INexusSso>();
            _nexusApi = components.Resolve<INexusApi>();
        }

        public async Task LoginToNexus()
        {
            IsLoggingIn = true;

            var nexusSso = _nexusSso.New();
            nexusSso.GrabbedKeyEvent += NexusSso_GrabbedKeyEvent;

            await nexusSso.ConnectAndGrabKeyAsync();
        }

        private void NexusSso_GrabbedKeyEvent(string key)
        {
            _nexusApi.Init(key);
            _nexusApi.IsPremiumUser();
        }

        public void ContinueOffline()
        {
            _viewController.IncrementCurrentViewIndex();
        }
    }
}
