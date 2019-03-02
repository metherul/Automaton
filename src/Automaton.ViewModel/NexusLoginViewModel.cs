using Autofac;
using Automaton.Model.Install.Intefaces;
using Automaton.Model.NexusApi.Interfaces;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;
using GalaSoft.MvvmLight.Command;

namespace Automaton.ViewModel
{
    public class NexusLoginViewModel : ViewModelBase, INexusLoginViewModel
    {
        private readonly IApiBase _apiBase;
        private readonly IInstallBase _installBase;
        private readonly IViewController _viewController;

        public RelayCommand LoginToNexusCommand => new RelayCommand(LoginToNexus);
        public RelayCommand ContinueOfflineCommand => new RelayCommand(ContinueOffline);

        public NexusLoginViewModel(IComponentContext components)
        {
            _apiBase = components.Resolve<IApiBase>();
            _installBase = components.Resolve<IInstallBase>();
            _viewController = components.Resolve<IViewController>();
        }

        public async void LoginToNexus()
        {
            _apiBase.HasLoggedInEvent += () =>
            {
                _viewController.IncrementCurrentViewIndex();
            };

            await _apiBase.InitializeAsync(_installBase.ModpackHeader.TargetGame);
        }

        public void ContinueOffline()
        {
            _viewController.IncrementCurrentViewIndex();
        }
    }
}
