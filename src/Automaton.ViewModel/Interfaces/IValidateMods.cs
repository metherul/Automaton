using System.Collections.ObjectModel;
using System.ComponentModel;
using Automaton.Model.ModpackBase;
using GalaSoft.MvvmLight.Command;

namespace Automaton.ViewModel.Interfaces
{
    public interface IValidateMods : IViewModel
    {
        RelayCommand ContinueOfflineCommand { get; set; }
        string CurrentArchiveMd5 { get; set; }
        string CurrentModName { get; set; }
        RelayCommand<Mod> FindAndValidateModFileCommand { get; set; }
        bool InitialValidationComplete { get; set; }
        RelayCommand InstallModpackCommand { get; set; }
        bool IsComputeMd5 { get; set; }
        bool IsLoggedIn { get; set; }
        bool IsLoginVisible { get; set; }
        string LogInButtonText { get; set; }
        ObservableCollection<Mod> MissingMods { get; set; }
        RelayCommand NexusLogInCommand { get; set; }
        bool NoMissingMods { get; set; }
        RelayCommand<Mod> OpenModSourceUrlCommand { get; set; }
        int TotalSourceFileCount { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}