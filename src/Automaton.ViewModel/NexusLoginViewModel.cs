using Automaton.ViewModel.Interfaces;
using GalaSoft.MvvmLight.Command;

namespace Automaton.ViewModel
{
    public class NexusLoginViewModel : ViewModelBase, INexusLoginViewModel
    {
        public RelayCommand LoginToNexusCommand => new RelayCommand(LoginToNexus);

        public void LoginToNexus()
        {

        }
    }
}
