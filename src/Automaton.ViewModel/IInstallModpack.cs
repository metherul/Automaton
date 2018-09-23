using System.ComponentModel;

namespace Automaton.ViewModel
{
    public interface IInstallModpack : IViewModel
    {
        string ConsoleOut { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}