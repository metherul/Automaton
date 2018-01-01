using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.ComponentModel;

namespace Automaton.ViewModel
{
    class GenericDialogViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand CloseDialogCommand { get; set; }

        public string Title { get; set; }
        public string Message { get; set; }

        public bool IsOpen { get; set; }

        public GenericDialogViewModel()
        {
            CloseDialogCommand = new RelayCommand(CloseDialog);

            IsOpen = false;

            Messenger.Default.Register<GenericDialogPayload>(this, RecievePayload);
        }

        private void RecievePayload(GenericDialogPayload payload)
        {
            if (!string.IsNullOrEmpty(payload.Title))
            {
                Title = payload.Title;
            }

            if (!string.IsNullOrEmpty(payload.Message))
            {
                Message = payload.Message;
            }
        }

        private void CloseDialog()
        {

        }
    }
}
