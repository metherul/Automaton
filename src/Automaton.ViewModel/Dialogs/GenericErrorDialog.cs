using System;
using System.ComponentModel;
using Automaton.ViewModel.Content.Dialogs.Interfaces;

namespace Automaton.ViewModel.Content.Dialogs
{
    public class GenericErrorDialog : IGenericErrorDialog, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string ErrorHeader { get; set; }
        public string ErrorMessage { get; set; }

        public bool IsFatal { get; set; }

        public async void DisplayParams(params object[] optionalParams)
        {
            IsFatal = Convert.ToBoolean(optionalParams[2]);
            ErrorHeader = (IsFatal ? "Fatal: " : "Error: ") + optionalParams[0]; 
            ErrorMessage = optionalParams[1].ToString();
        }
    }
}
