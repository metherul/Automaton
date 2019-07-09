using System.ComponentModel;
using Autofac;
using Automaton.ViewModel.Dialogs.Interfaces;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.Model.HandyUtils.Interfaces;

namespace Automaton.ViewModel.Controllers
{
    public class DialogController : IDialogController, INotifyPropertyChanged
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly IDialogRedirector _dialogRedirector;

        public event PropertyChangedEventHandler PropertyChanged;
        public int CurrentIndex { get; set; }
        public bool IsDialogOpen { get; set; }
        
        public DialogController(IComponentContext components)
        {
            _lifetimeScope = components.Resolve<ILifetimeScope>();
            _dialogRedirector = components.Resolve<IDialogRedirector>();

            _dialogRedirector.RoutedLogEvent += _dialogRedirector_RoutedLogEvent;
        }

        private void _dialogRedirector_RoutedLogEvent(string message)
        {
            OpenLogDialog(message);
        }

        public void CloseCurrentDialog()
        {
            IsDialogOpen = false;
        }

        public void OpenErrorDialog(bool isFatal, string header, string message)
        {
            IsDialogOpen = true;
            CurrentIndex = (int)DialogType.GenericErrorDialog;

            _lifetimeScope.Resolve<IGenericErrorDialog>().DisplayParams(isFatal, header, message);
        }

        public void OpenLogDialog(string message)
        {
            IsDialogOpen = true;
            CurrentIndex = (int)DialogType.GenericLogDialog;

            _lifetimeScope.Resolve<IGenericLogDialog>().DisplayParams(message);
        }

        public void OpenLoadingDialog()
        {
            IsDialogOpen = true;
            CurrentIndex = (int)DialogType.GenericLoadingDialog;
        }
    }

    public enum DialogType
    {
        GenericErrorDialog,
        GenericLogDialog,
        GenericLoadingDialog
    }
}
