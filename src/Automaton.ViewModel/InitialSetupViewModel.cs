using System.Collections.Generic;
using System.IO;
using Autofac;
using Automaton.Model.Install.Intefaces;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;
using Automaton.ViewModel.Utilities.Interfaces;
using GalaSoft.MvvmLight.Command;

namespace Automaton.ViewModel
{
    public class InitialSetupViewModel : ViewModelBase, IInitialSetupViewModel
    {
        private readonly IViewController _viewController;
        private readonly IFileSystemBrowser _fileSystemBrowser;
        private readonly IInstallBase _installBase;

        public RelayCommand OpenInstallFolderCommand { get => new RelayCommand(OpenInstallFolder); }
        public RelayCommand OpenDownloadsFolderCommand {get => new RelayCommand(OpenDownloadsFolder); }
        public RelayCommand IncrementViewIndexCommand { get => new RelayCommand(IncrementViewIndex); }

        public string InstallLocation { get; set; }
        public string DownloadsLocation { get; set; }

        public InitialSetupViewModel(IComponentContext components)
        {
            _viewController = components.Resolve<IViewController>();
            _fileSystemBrowser = components.Resolve<IFileSystemBrowser>();
            _installBase = components.Resolve<IInstallBase>();
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
                _installBase.InstallDirectory = InstallLocation;
                _installBase.SourceDirectories = new List<string>() {DownloadsLocation};

                _viewController.IncrementCurrentViewIndex();
            }
        }
    }
}
