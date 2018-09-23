using GalaSoft.MvvmLight.Command;

namespace Automaton.ViewModel.Interfaces
{
    public interface ILoadModpack : IViewModel
    {
        RelayCommand LoadModpackCommand { get; set; }
    }
}