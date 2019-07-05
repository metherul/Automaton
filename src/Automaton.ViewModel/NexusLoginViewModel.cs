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

            NexusSso_GrabbedKeyEvent("UDZNZWNoR3A3MkNyZXdNcExBc0JpdzVTbE5UenBWSVZKOElFWi8yeUNXRWREV3FWbjBOUW1JUGcrV3JlS0c4RS0tdE5CK2ZQZmFXNytNL2tYcXBnalNQQT09--53065a9dd5fd2880be1b882d3947b0c96de2864d");

            //var nexusSso = _nexusSso.New();
            //nexusSso.GrabbedKeyEvent += NexusSso_GrabbedKeyEvent;

            //await nexusSso.ConnectAndGrabKeyAsync();
        }

        private void NexusSso_GrabbedKeyEvent(string key)
        {
            _nexusApi.Init(key);

            _viewController.IncrementCurrentViewIndex();
        }

        public void ContinueOffline()
        {
            _viewController.IncrementCurrentViewIndex();
        }
    }
}
