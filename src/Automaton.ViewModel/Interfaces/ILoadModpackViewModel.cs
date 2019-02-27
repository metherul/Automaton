using Automaton.ViewModel.Content.Interfaces;
using GalaSoft.MvvmLight.Command;

namespace Automaton.ViewModel.Interfaces
{
    public interface ILoadModpackViewModel : IViewModel
    {
        RelayCommand ChooseModpackCommand { get; set; }
    }
}