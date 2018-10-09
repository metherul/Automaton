using System;

namespace Automaton.ViewModel.Controllers.Interfaces
{
    public interface IViewController : IController
    {
        event EventHandler<int> ViewIndexChangedEvent;

        int CurrentViewIndex { get; set; }

        void IncrementCurrentViewIndex();
    }
}