using Autofac;
using Automaton.Model.Interfaces;
using Automaton.ViewModel.Controllers;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;
using Automaton.ViewModel.Utilities.Interfaces;
using GalaSoft.MvvmLight.Command;

namespace Automaton.ViewModel
{
    public class ValidateModsViewModel : ViewModelBase, IValidateModsViewModel
    {
        private readonly IViewController _viewController;
        private readonly IFileSystemBrowser _fileSystemBrowser;
        private readonly IDialogController _dialogController;
        private readonly ILogger _logger;

        public RelayCommand ScanDirectoryCommand => new RelayCommand(ScanDirectory);
        //public RelayCommand<ExtendedMod> OpenModSourceUrlCommand => new RelayCommand<ExtendedMod>(OpenNexusLink);
        //public RelayCommand<ExtendedMod> FindAndValidateModCommand => new RelayCommand<ExtendedMod>(FindAndValidateMod);
        //public RelayCommand<ExtendedMod> OpenNexusLinkCommand => new RelayCommand<ExtendedMod>(OpenNexusLink);

        //public ObservableCollection<ExtendedMod> ModsList { get; set; } = new ObservableCollection<ExtendedMod>();
        
        public ValidateModsViewModel(IComponentContext components)
        {
            _viewController = components.Resolve<IViewController>();
            _fileSystemBrowser = components.Resolve<IFileSystemBrowser>();
            _dialogController = components.Resolve<IDialogController>();

            _viewController.ViewIndexChangedEvent += ViewControllerOnViewIndexChangedEvent;
            //_nxmHandle.RecievedPipedDataEvent += NxmHandle_RecievedPipedDataEvent;
        }

        private void ViewControllerOnViewIndexChangedEvent(object sender, int e) // Event occurs upon load of this view
        {
            if (e != (int)ViewIndex.ValidateMods)
            {
                return;
            }
        }


        private void InitializeAutoDownloader()
        {
        }

        private async void ValidateMods(string directoryPath = "")
        {
        }

        private void ScanDirectory()
        {
        }

        private void OpenNexusLink()
        {
        }
    }
}
