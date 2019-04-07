using Autofac;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Dialogs.Interfaces;
using GalaSoft.MvvmLight.Command;

namespace Automaton.ViewModel.Dialogs
{
    public class GenericLogDialog : ViewModelBase, IGenericLogDialog
    {
        private readonly IDialogController _dialogController;

        public RelayCommand CloseDialogCommand => new RelayCommand(CloseDialog);

        public string Message { get; set; }

        public GenericLogDialog(IComponentContext components)
        {
            _dialogController = components.Resolve<IDialogController>();
        }

        public void DisplayParams(string message)
        {
            Message = message;
        }

        private void CloseDialog()
        {
            _dialogController.CloseCurrentDialog();
        }
    }
}
