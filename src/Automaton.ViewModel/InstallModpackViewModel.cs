using Autofac;
using Automaton.ViewModel.Controllers;
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

        public ObservableCollection<string> DebugOutput { get; set; } = new ObservableCollection<string>();

        public RelayCommand InstallModpackCommand => new RelayCommand(InstallModpack);

        public bool IsInstalling { get; set; }
        public int TotalModCount { get; set; }

        public InstallModpackViewModel(IComponentContext components)
        {
            _viewController = components.Resolve<IViewController>();

            _viewController.ViewIndexChangedEvent += _viewController_ViewIndexChangedEvent;
        }

        private void _viewController_ViewIndexChangedEvent(object sender, int e)
        {
            if (e != (int)ViewIndex.InstallModpack)
            {
                return;
            }
        }

        private void DebugLogCallback(object sender, string e)
        {
            if (e == "_END")
            {
                IsInstalling = false;
                _viewController.IncrementCurrentViewIndex();
                return;
            }

            Application.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                DebugOutput.Insert(0, e);
                TotalModCount--;
            });
        }
    
        private void InstallModpack()
        {
            IsInstalling = true;

            // Install modpack here
        }
    }
}
