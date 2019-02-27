using System;
using System.ComponentModel;
using Autofac;
using Automaton.ViewModel.Content.Dialogs.Interfaces;
using Automaton.ViewModel.Controllers.Interfaces;

namespace Automaton.ViewModel.Controllers
{
    public class DialogController : IDialogController, INotifyPropertyChanged
    {
        private readonly ILifetimeScope _lifetimeScope;

        public event PropertyChangedEventHandler PropertyChanged;

        public int CurrentIndex { get; set; }
        public bool IsDialogOpen { get; set; }

        public DialogController(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public void OpenDialog(DialogType dialogType, params object[] parameterObjects)
        {
            CurrentIndex = Convert.ToInt32(dialogType);
            IsDialogOpen = true;

            switch (dialogType)
            {
                case (DialogType.GenericErrorDialog):
                    _lifetimeScope.Resolve<IGenericErrorDialog>().DisplayParams(parameterObjects);
                    break;
            }
        }
    }

    public enum DialogType
    {
        GenericErrorDialog
    }
}
