using Autofac;
using Automaton.ViewModel.Controllers.Interfaces;
using System.ComponentModel;

namespace Automaton.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
