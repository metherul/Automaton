using Automaton.Handles;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.ComponentModel;

namespace Automaton.ViewModel
{
    class GenericErrorDialogViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand CloseDialogCommand { get; set; }

        public string Title { get; set; }
        public string ErrorMessage { get; set; }

        public bool IsOpen { get; set; }

        public GenericErrorDialogViewModel()
        {
            CloseDialogCommand = new RelayCommand(CloseDialog);

            IsOpen = false;

            Messenger.Default.Register<GenericErrorDialogPayload>(this, RecievePayload);
        }

        private void RecievePayload(GenericErrorDialogPayload payload)
        {
            if (!string.IsNullOrEmpty(payload.Title))
            {
                Title = payload.Title;
            }

            if (!string.IsNullOrEmpty(payload.ErrorMessage))
            {
                ErrorMessage = payload.ErrorMessage;
            }

            IsOpen = true;
        }

        private void CloseDialog()
        {
            Title = "";
            ErrorMessage = "";

            IsOpen = false;
        }
    }
}
