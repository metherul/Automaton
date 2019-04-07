using System.ComponentModel;
using System.Windows;
using Automaton.ViewModel.Interfaces;
using GalaSoft.MvvmLight.Command;

namespace Automaton.ViewModel.Interfaces
{
    public interface IMainWindowViewModel : IViewModel
    {
        RelayCommand<Window> CloseWindowCommand { get; set; }
        int CurrentTransitionerIndex { get; set; }
        RelayCommand<Window> MinimizeWindowCommand { get; set; }
        RelayCommand<Window> MoveWindowCommand { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}