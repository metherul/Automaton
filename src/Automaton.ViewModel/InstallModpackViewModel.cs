using Autofac;
using Automaton.Model.Install.Intefaces;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;

namespace Automaton.ViewModel
{
    public class InstallModpackViewModel : ViewModelBase, IInstallModpackViewModel
    {
        private readonly IViewController _viewController;
        private readonly IInstallBase _installBase;

        public ObservableCollection<string> DebugOutput => new ObservableCollection<string>();

        public RelayCommand InstallModpackCommand => new RelayCommand(InstallModpack);

        public bool IsInstalling { get; set; }

        public InstallModpackViewModel(IComponentContext components)
        {
            _viewController = components.Resolve<IViewController>();
            _installBase = components.Resolve<IInstallBase>();
        }

        private void InstallModpack()
        {

        }
    }
}
