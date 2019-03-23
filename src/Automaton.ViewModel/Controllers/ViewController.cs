using System;
using Automaton.ViewModel.Controllers.Interfaces;

namespace Automaton.ViewModel.Controllers
{
    public class ViewController : IViewController
    {
        public event EventHandler<int> ViewIndexChangedEvent;

        public int CurrentViewIndex { get; set; } = 0;

        public void IncrementCurrentViewIndex()
        {
            CurrentViewIndex++;

            ViewIndexChangedEvent(this, CurrentViewIndex);
        }
    }
}