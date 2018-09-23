using System;

namespace Automaton.ViewModel.Controllers
{
    public interface IViewController : IController
    {
        event EventHandler<int> ViewIndexChangedEvent;

        int CurrentViewIndex { get; set; }

        void IncrementCurrentViewIndex();
    }
}