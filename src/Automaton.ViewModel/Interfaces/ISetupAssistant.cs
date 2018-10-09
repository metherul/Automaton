using System.Collections.ObjectModel;
using System.ComponentModel;
using Automaton.Model.ModpackBase;
using GalaSoft.MvvmLight.Command;

namespace Automaton.ViewModel.Interfaces
{
    public interface ISetupAssistant : IViewModel
    {
        string Description { get; set; }
        string ImagePath { get; set; }
        RelayCommand IncrementCurrentViewIndexCommand { get; set; }
        ObservableCollection<Group> SetupAssistantGroup { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}