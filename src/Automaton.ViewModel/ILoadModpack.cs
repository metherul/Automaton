using GalaSoft.MvvmLight.Command;

namespace Automaton.ViewModel
{
    public interface ILoadModpack : IViewModel
    {
        RelayCommand LoadModpackCommand { get; set; }
    }
}