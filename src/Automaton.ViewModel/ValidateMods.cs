using System.ComponentModel;
using System.Runtime.CompilerServices;
using Automaton.Model.Annotations;

namespace Automaton.ViewModel
{
    public class ValidateMods : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

