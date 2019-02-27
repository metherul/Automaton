using Automaton.Model.Modpack.Interfaces;
using Automaton.ViewModel.Controllers;
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
        private readonly IModpackRead _modpackRead;
        private readonly IDialogController _dialogController;

        public RelayCommand ChooseModpackCommand { get; set; }

        public LoadModpackViewModel(IViewController viewController, IFileSystemBrowser filesystemBrowser, IModpackRead modpackRead, IDialogController dialogController)
        {
            _viewController = viewController;
            _filesystemBrowser = filesystemBrowser;
            _modpackRead = modpackRead;
            _dialogController = dialogController;

            ChooseModpackCommand = new RelayCommand(ChooseModpack);
        }

        private async void ChooseModpack()
        {
            var modpackPath = await _filesystemBrowser.OpenFileBrowserAsync("Automaton Modpacks (*.auto, *.7z, *.rar, *.zip) | *.auto; *.7z; *.rar; *.zip|All Files (*.*)|*.*", "Select an Automaton Modpack");

            if (string.IsNullOrEmpty(modpackPath))
            {
                return;
            }

            var (isSuccessful, errorMessage) = await _modpackRead.LoadModpackAsync(modpackPath);

            if (!isSuccessful)
            {
                _dialogController.OpenDialog(DialogType.GenericErrorDialog, "Modpack Read", errorMessage, true);
                return;
            }

            _viewController.IncrementCurrentViewIndex();
        }
    }
}
