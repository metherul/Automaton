using Autofac;
using Automaton.Model.Install.Intefaces;
using Automaton.Model.Install.Interfaces;
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
        private readonly IInstallModpack _installModpack;
        private readonly IInstallBase _installBase;

        public ObservableCollection<string> DebugOutput { get; set; } = new ObservableCollection<string>();

        public RelayCommand InstallModpackCommand => new RelayCommand(InstallModpack);

        public bool IsInstalling { get; set; }
        public int TotalModCount { get; set; }

        public InstallModpackViewModel(IComponentContext components)
        {
            _viewController = components.Resolve<IViewController>();
            _installModpack = components.Resolve<IInstallModpack>();
            _installBase = components.Resolve<IInstallBase>();

            _viewController.ViewIndexChangedEvent += _viewController_ViewIndexChangedEvent;
            _installModpack.DebugLogCallback += DebugLogCallback;
        }

        private void _viewController_ViewIndexChangedEvent(object sender, int e)
        {
            if (e != (int)ViewIndex.InstallModpack)
            {
                return;
            }

            TotalModCount = _installBase.ModpackMods.Count;
        }

        private void DebugLogCallback(object sender, string e)
        {
            if (e == "_CLEAR")
            {
                Application.Current.Dispatcher.BeginInvoke((Action)delegate
                {
                    DebugOutput = new ObservableCollection<string>();
                });

                TotalModCount--;

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
                if ((_installBase.ModpackMods.Count - TotalModCount) % 30 == 0)
                {
                    DebugOutput = new ObservableCollection<string>();
                }

                DebugOutput.Insert(0, e);
                TotalModCount--;
            });
        }
    
        private void InstallModpack()
        {
            IsInstalling = true;

            Task.Factory.StartNew(_installModpack.Install);
        }
    }
}
