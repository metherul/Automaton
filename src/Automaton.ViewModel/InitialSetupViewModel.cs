using System;
using Autofac;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;
using Automaton.ViewModel.Utilities.Interfaces;
using GalaSoft.MvvmLight.Command;
using Alphaleonis.Win32.Filesystem;
using Automaton.Model.Interfaces;

namespace Automaton.ViewModel
{
    public class InitialSetupViewModel : ViewModelBase, IInitialSetupViewModel
    {
        private readonly IViewController _viewController;
        private readonly IFileSystemBrowser _fileSystemBrowser;
        private readonly ILifetimeData _lifetimeData;

        public RelayCommand OpenInstallFolderCommand { get => new RelayCommand(OpenInstallFolder); }
        public RelayCommand OpenDownloadsFolderCommand {get => new RelayCommand(OpenDownloadsFolder); }
        public RelayCommand IncrementViewIndexCommand { get => new RelayCommand(IncrementViewIndex); }

        public string InstallLocation { get; set; }
        public string DownloadsLocation { get; set; }

        public InitialSetupViewModel(IComponentContext components)
        {
            _viewController = components.Resolve<IViewController>();
            _fileSystemBrowser = components.Resolve<IFileSystemBrowser>();

            _lifetimeData = components.Resolve<ILifetimeData>();
        }

        public void OpenInstallFolder()
        {
             InstallLocation = _fileSystemBrowser.OpenDirectoryBrowser("The location where the modpack will be installed.");
        }

        private void OpenDownloadsFolder()
        {
            DownloadsLocation = _fileSystemBrowser.OpenDirectoryBrowser("The location where mods will be downloaded to.");
        }

        private void IncrementViewIndex()
        {
            if (Directory.Exists(InstallLocation) && Directory.Exists(DownloadsLocation))
            {
                _lifetimeData.InstallPath = InstallLocation;
                _lifetimeData.DownloadPath = DownloadsLocation;

                _viewController.IncrementCurrentViewIndex();
            }
        }
    }
}
