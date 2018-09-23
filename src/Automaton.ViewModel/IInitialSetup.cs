using System.ComponentModel;
using GalaSoft.MvvmLight.Command;

namespace Automaton.ViewModel
{
    public interface IInitialSetup : IViewModel
    {
        bool CanContinue { get; set; }
        string Description { get; set; }
        string DownloadsDirectory { get; set; }
        RelayCommand IncrementCurrentViewIndexCommand { get; set; }
        string InstallDirectory { get; set; }
        string ModpackName { get; set; }
        RelayCommand OpenDownloadsFolderCommand { get; set; }
        RelayCommand OpenInstallFolderCommand { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}