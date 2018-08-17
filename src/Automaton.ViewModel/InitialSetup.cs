using System.ComponentModel;
using System.IO;
using Automaton.ViewModel.Controllers;
using Automaton.Model.Utility;
using GalaSoft.MvvmLight.Command;
using Ookii.Dialogs.Wpf;

namespace Automaton.ViewModel
{
    public class InitialSetup : ViewController, INotifyPropertyChanged
    {
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

        public InitialSetup()
        {
            OpenInstallFolderCommand = new RelayCommand(OpenInstallFolder);
            OpenDownloadsFolderCommand = new RelayCommand(OpenDownloadsFolder);

            IncrementCurrentViewIndexCommand = new RelayCommand(IncrementCurrentViewIndex);

            Modpack.ModpackLoadedEvent += ModpackLoaded;
        }

        private void ModpackLoaded()
        {
            ModpackName = Model.Instance.Automaton.ModpackHeader.ModpackName;
            Description = Model.Instance.Automaton.ModpackHeader.Description;
        }

        private void OpenInstallFolder()
        {
            InstallDirectory = OpenDirectoryBrowser();
        }

        private void OpenDownloadsFolder()
        {
            DownloadsDirectory = OpenDirectoryBrowser();
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

