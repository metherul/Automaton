using System;
using System.ComponentModel;
using System.Windows;
using Automaton.ViewModel.Content.Dialogs.Interfaces;
using GalaSoft.MvvmLight.Command;

namespace Automaton.ViewModel.Content.Dialogs
{
    public class GenericErrorDialog : IGenericErrorDialog, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand<Window> CloseWindowCommand => new RelayCommand<Window>(CloseWindow);

        public string ErrorHeader { get; set; }
        public string ErrorMessage { get; set; }

        public bool IsFatal { get; set; }

        public async void DisplayParams(bool isFatal, string header, string message)
        {
            IsFatal = isFatal;
            ErrorHeader = (IsFatal ? "Fatal: " : "Error: ") + header; 
            ErrorMessage = message;
        }

        private static void CloseWindow(Window window)
        {
            window.Close();
        }
    }
}
