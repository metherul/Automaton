using System.ComponentModel;
using System.Windows;
using Automaton.ViewModel.Dialogs.Interfaces;
using GalaSoft.MvvmLight.Command;

namespace Automaton.ViewModel.Dialogs
{
    public class GenericErrorDialog : IGenericErrorDialog, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand<Window> CloseWindowCommand => new RelayCommand<Window>(CloseWindow);

        public string ErrorHeader { get; set; }
        public string ErrorMessage { get; set; }

        public bool IsFatal { get; set; }

        public void DisplayParams(bool isFatal, string header, string message)
        {
            IsFatal = isFatal;
            ErrorHeader = (IsFatal ? "Fatal: " : "Error: ") + header; 
            ErrorMessage = message;
        }

        private void CloseWindow(Window window)
        {
            window.Close();
        }
    }
}
