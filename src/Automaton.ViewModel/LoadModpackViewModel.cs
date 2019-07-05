using Autofac;
using Automaton.Model.Interfaces;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;
using Automaton.ViewModel.Utilities;
using Automaton.ViewModel.Utilities.Interfaces;
using GalaSoft.MvvmLight.Command;
using System.Threading.Tasks;

namespace Automaton.ViewModel
{
    public class LoadModpackViewModel : ILoadModpackViewModel
    {
        private readonly IViewController _viewController;
        private readonly IFileSystemBrowser _filesystemBrowser;
        private readonly IDialogController _dialogController;
        private readonly ILoadModpack _loadModpack;

        public AsyncCommand ChooseModpackCommand { get => new AsyncCommand(ChooseModpack); }

        public LoadModpackViewModel(IComponentContext components)
        {
            _viewController = components.Resolve<IViewController>();
            _filesystemBrowser = components.Resolve<IFileSystemBrowser>(); 
            _dialogController = components.Resolve<IDialogController>();

            _loadModpack = components.Resolve<ILoadModpack>();
        }

        private async Task ChooseModpack()
        {
            var modpackPath = await _filesystemBrowser.OpenFileBrowserAsync("Automaton Modpacks (*.auto, *.7z, *.rar, *.zip) | *.auto; *.7z; *.rar; *.zip|All Files (*.*)|*.*", "Select an Automaton Modpack");

            if (!string.IsNullOrEmpty(modpackPath))
            {
                _dialogController.OpenLoadingDialog();

                await _loadModpack.LoadAsync(modpackPath);

                // Apply theme
                ApplyTheme();

                _dialogController.CloseCurrentDialog();
                _viewController.IncrementCurrentViewIndex();
            }
        }

        private void ApplyTheme()
        {
        }
    }
}
