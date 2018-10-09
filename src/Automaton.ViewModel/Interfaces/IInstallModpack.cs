using System.ComponentModel;

namespace Automaton.ViewModel.Interfaces
{
    public interface IInstallModpack : IViewModel
    {
        string ConsoleOut { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}