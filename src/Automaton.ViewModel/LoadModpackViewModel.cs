using Autofac;
using Automaton.Model.Interfaces;
using Automaton.Model.Modpack.Interfaces;
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
        private readonly ILogger _logger;

        public RelayCommand ChooseModpackCommand { get => new RelayCommand(ChooseModpack); }

        public LoadModpackViewModel(IComponentContext components)
        {
            _viewController = components.Resolve<IViewController>();
            _filesystemBrowser = components.Resolve<IFileSystemBrowser>(); 
            _modpackRead = components.Resolve<IModpackRead>(); 
            _dialogController = components.Resolve<IDialogController>();
            _logger = components.Resolve<ILogger>();
        }

        private async void ChooseModpack()
        {
            _logger.WriteLine("Opening modpack filesystemBrowser");

            var modpackPath = await _filesystemBrowser.OpenFileBrowserAsync("Automaton Modpacks (*.auto, *.7z, *.rar, *.zip) | *.auto; *.7z; *.rar; *.zip|All Files (*.*)|*.*", "Select an Automaton Modpack");

            if (string.IsNullOrEmpty(modpackPath))
            {
                _logger.WriteLine("modpackPath is null or empty");

                return;
            }

            var isSuccessful = await _modpackRead.LoadModpackAsync(modpackPath);

            if (!isSuccessful)
            {
                _logger.WriteLine("Modpack load was not successful");

                return;
            }

            _viewController.IncrementCurrentViewIndex();
        }
    }
}
