using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Automaton.Model.ModpackBase;
using Automaton.Model.ModpackBase.Interfaces;
using GalaSoft.MvvmLight.Command;

namespace Automaton.ViewModel.Interfaces
{
    public interface ISetupAssistant : IViewModel
    {
        string Description { get; set; }
        string ImagePath { get; set; }
        RelayCommand IncrementCurrentViewIndexCommand { get; set; }
        ObservableCollection<IGroup> SetupAssistantGroup { get; set; }

        event PropertyChangedEventHandler PropertyChanged;

        void ControlChecked(dynamic sender, RoutedEventArgs e);
        void ControlHover(dynamic sender, RoutedEventArgs e);
        void ControlUnchecked(dynamic sender, RoutedEventArgs e);
    }
}