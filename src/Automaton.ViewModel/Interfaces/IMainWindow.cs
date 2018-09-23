using System.ComponentModel;
using System.Windows;
using GalaSoft.MvvmLight.Command;

namespace Automaton.ViewModel.Interfaces
{
    public interface IMainWindow : IViewModel
    {
        RelayCommand<Window> CloseWindowCommand { get; set; }
        int CurrentTransitionerIndex { get; set; }
        RelayCommand<Window> MinimizeWindowCommand { get; set; }
        RelayCommand<Window> MoveWindowCommand { get; set; }

        event PropertyChangedEventHandler PropertyChanged;

        void ModpackLoaded();
    }
}