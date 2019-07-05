using Autofac;
using Automaton.Model.Interfaces;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;
using Automaton.ViewModel.Utilities.Interfaces;
using GalaSoft.MvvmLight.Command;

namespace Automaton.ViewModel
{
    public class LoadModpackViewModel : ILoadModpackViewModel
    {
        private readonly IViewController _viewController;
        private readonly IFileSystemBrowser _filesystemBrowser;
        private readonly IDialogController _dialogController;
        private readonly ILoadModpack _loadModpack;

        public RelayCommand ChooseModpackCommand { get => new RelayCommand(ChooseModpack); }

        public LoadModpackViewModel(IComponentContext components)
        {
            _viewController = components.Resolve<IViewController>();
            _filesystemBrowser = components.Resolve<IFileSystemBrowser>(); 
            _dialogController = components.Resolve<IDialogController>();

            _loadModpack = components.Resolve<ILoadModpack>();
        }

        private async void ChooseModpack()
        {
            var modpackPath = await _filesystemBrowser.OpenFileBrowserAsync("Automaton Modpacks (*.auto, *.7z, *.rar, *.zip) | *.auto; *.7z; *.rar; *.zip|All Files (*.*)|*.*", "Select an Automaton Modpack");

            if (string.IsNullOrEmpty(modpackPath))
            {
                return;
            }

            _loadModpack.Load(modpackPath);

            // Apply theme
            ApplyTheme();

            _viewController.IncrementCurrentViewIndex();
        }

        private void ApplyTheme()
        {
        }
    }
}
