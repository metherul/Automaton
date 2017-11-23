using Automaton.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using System.ComponentModel;
using System.Windows.Forms;

namespace Automaton.ViewModel
{
    class DialogHandler : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand OpenMainDialogCommand { get; set; }

        public string PackLocation { get; set; }

        public DialogHandler()
        {
            OpenMainDialogCommand = new RelayCommand(OpenMainDialog);
        }

        private async void OpenMainDialog()
        {

        }
    }
}
