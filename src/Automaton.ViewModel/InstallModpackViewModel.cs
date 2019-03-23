using Autofac;
using Automaton.Model.Install.Interfaces;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace Automaton.ViewModel
{
    public class InstallModpackViewModel : ViewModelBase, IInstallModpackViewModel
    {
        private readonly IViewController _viewController;
        private readonly IInstallModpack _installModpack;

        public ObservableCollection<string> DebugOutput { get; set; } = new ObservableCollection<string>();

        public RelayCommand InstallModpackCommand => new RelayCommand(InstallModpack);

        public bool IsInstalling { get; set; }

        public InstallModpackViewModel(IComponentContext components)
        {
            _viewController = components.Resolve<IViewController>();
            _installModpack = components.Resolve<IInstallModpack>();

            _installModpack.DebugLogCallback += DebugLogCallback;
        }

        private void DebugLogCallback(object sender, string e)
        {
            if (e == "_CLEAR")
            {
                DebugOutput = new ObservableCollection<string>();
                return;
            }

            if (e == "_END")
            {
                IsInstalling = false;
                _viewController.IncrementCurrentViewIndex();
                return;
            }

            Application.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                DebugOutput.Add(e);
            });
        }
    
        private void InstallModpack()
        {
            IsInstalling = true;

            Task.Factory.StartNew(_installModpack.Install);
        }
    }
}
