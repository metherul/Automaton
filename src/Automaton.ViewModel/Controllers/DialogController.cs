using System.ComponentModel;
using System.Runtime.ExceptionServices;
using Autofac;
using Automaton.Model.Interfaces;
using Automaton.ViewModel.Dialogs.Interfaces;
using Automaton.ViewModel.Controllers.Interfaces;

namespace Automaton.ViewModel.Controllers
{
    public class DialogController : IDialogController, INotifyPropertyChanged
    {
        private readonly ILifetimeScope _lifetimeScope;

        public event PropertyChangedEventHandler PropertyChanged;

        public int CurrentIndex { get; set; }
        public bool IsDialogOpen { get; set; }
        
        public DialogController(IComponentContext components)
        {
            _lifetimeScope = components.Resolve<ILifetimeScope>();
        }

        public void CloseCurrentDialog()
        {
            IsDialogOpen = false;
        }

        private void Logger_CapturedError(object sender, FirstChanceExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                return;
            }

            if (e.Exception.Message.Contains("materialDesign")) // Annoying XAML binding error. Can't fix.
            {
                return;
            }

            OpenErrorDialog(true, "Error", e.Exception.Message);
        }

        private void Logger_CapturedLog(object sender, string e)
        {
            OpenLogDialog(e);
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
