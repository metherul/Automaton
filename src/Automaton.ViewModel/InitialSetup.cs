using Automaton.Model.Utility;
using Automaton.ViewModel.Controllers;
using GalaSoft.MvvmLight.Command;
using Ookii.Dialogs.Wpf;
using System.ComponentModel;
using System.IO;
using Automaton.Model.Instance.Interfaces;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;

namespace Automaton.ViewModel
{
    public class InitialSetup : IInitialSetup, INotifyPropertyChanged
    {
        private readonly IViewController _viewController;
        private readonly IAutomatonInstance _automatonInstance;

        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand OpenInstallFolderCommand { get; set; }
        public RelayCommand OpenDownloadsFolderCommand { get; set; }

        public RelayCommand IncrementCurrentViewIndexCommand { get; set; }

        private string _installDirectory;
        public string InstallDirectory
        {
            get => _installDirectory;
            set
            {
                _installDirectory = value;

                CanContinue = Directory.Exists(_installDirectory) && Directory.Exists(_downloadsDirectory);
            }
        }

        private string _downloadsDirectory;
        public string DownloadsDirectory
        {
            get => _downloadsDirectory;
            set
            {
                _downloadsDirectory = value;

                CanContinue = Directory.Exists(_installDirectory) && Directory.Exists(_downloadsDirectory);
            }
        }

        public string ModpackName { get; set; }
        public string Description { get; set; }

        public bool CanContinue { get; set; }

        public InitialSetup(IViewController viewController, IAutomatonInstance automatonInstance)
        {
            _viewController = viewController;
            _automatonInstance = automatonInstance;

            OpenInstallFolderCommand = new RelayCommand(OpenInstallFolder);
            OpenDownloadsFolderCommand = new RelayCommand(OpenDownloadsFolder);

            IncrementCurrentViewIndexCommand = new RelayCommand(_viewController.IncrementCurrentViewIndex);
        }

        private void ModpackLoaded()
        {
            ModpackName = _automatonInstance.ModpackHeader.ModpackName;
            Description = _automatonInstance.ModpackHeader.Description;
        }

        private void OpenInstallFolder()
        {
            InstallDirectory = OpenDirectoryBrowser();
            _automatonInstance.InstallLocation = InstallDirectory;
        }

        private void OpenDownloadsFolder()
        {
            DownloadsDirectory = OpenDirectoryBrowser();
            _automatonInstance.SourceLocation = DownloadsDirectory;
        }

        private string OpenDirectoryBrowser()
        {
            var directoryDialog = new VistaFolderBrowserDialog();

            if (directoryDialog.ShowDialog() == true)
            {
                return directoryDialog.SelectedPath;
            }

            return null;
        }
    }
}