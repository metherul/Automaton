using Autofac;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;
using Automaton.ViewModel.Utilities.Interfaces;
using GalaSoft.MvvmLight.Command;
using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace Automaton.ViewModel
{
    public class LoadModpackViewModel : ILoadModpackViewModel
    {
        private readonly IViewController _viewController;
        private readonly IFileSystemBrowser _filesystemBrowser;
        private readonly IDialogController _dialogController;

        public RelayCommand ChooseModpackCommand { get => new RelayCommand(ChooseModpack); }

        public LoadModpackViewModel(IComponentContext components)
        {
            _viewController = components.Resolve<IViewController>();
            _filesystemBrowser = components.Resolve<IFileSystemBrowser>(); 
            _dialogController = components.Resolve<IDialogController>();
        }

        private async void ChooseModpack()
        {
            var modpackPath = await _filesystemBrowser.OpenFileBrowserAsync("Automaton Modpacks (*.auto, *.7z, *.rar, *.zip) | *.auto; *.7z; *.rar; *.zip|All Files (*.*)|*.*", "Select an Automaton Modpack");

            if (string.IsNullOrEmpty(modpackPath))
            {
                return;
            }

            // Apply theme
            ApplyTheme();

            _viewController.IncrementCurrentViewIndex();
        }

        private void ApplyTheme()
        {
        }
    }
}
