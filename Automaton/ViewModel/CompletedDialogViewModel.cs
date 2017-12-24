using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Automaton.ViewModel
{
    class CompletedDialogViewModel
    {
        public RelayCommand CloseCardCommand { get; set; }

        public CompletedDialogViewModel()
        {
            CloseCardCommand = new RelayCommand(CloseCard);
        }

        public void CloseCard()
        {
            Messenger.Default.Send(CardControl.IsCardOpen);
        }
    }
}
