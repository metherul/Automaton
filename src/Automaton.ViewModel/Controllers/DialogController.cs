using System;
using System.ComponentModel;
using System.Runtime.ExceptionServices;
using Autofac;
using Automaton.Model.Interfaces;
using Automaton.ViewModel.Content.Dialogs.Interfaces;
using Automaton.ViewModel.Controllers.Interfaces;

namespace Automaton.ViewModel.Controllers
{
    public class DialogController : IDialogController, INotifyPropertyChanged
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly ILogger _logger;

        public event PropertyChangedEventHandler PropertyChanged;

        public int CurrentIndex { get; set; }
        public bool IsDialogOpen { get; set; }

        public DialogController(IComponentContext components)
        {
            _lifetimeScope = components.Resolve<ILifetimeScope>();
            _logger = components.Resolve<ILogger>();

            _logger.CapturedError += Logger_CapturedError;
        }

        private void Logger_CapturedError(object sender, FirstChanceExceptionEventArgs e)
        {
            if (e.Exception.Message.Contains("materialDesign")) // Annoying XAML binding error. Can't fix.
            {
                return;
            }

            OpenErrorDialog(true, "Error", e.Exception.Message);
        }

        public void OpenErrorDialog(bool isFatal, string header, string message)
        {
            IsDialogOpen = true;

            _lifetimeScope.Resolve<IGenericErrorDialog>().DisplayParams(isFatal, header, message);
        }
    }

    public enum DialogType
    {
        GenericErrorDialog
    }
}
