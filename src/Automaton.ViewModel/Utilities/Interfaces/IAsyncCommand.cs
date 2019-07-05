using System.Threading.Tasks;
using System.Windows.Input;

namespace Automaton.ViewModel.Utilities.Interfaces
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync();
        bool CanExecute();
    }

}
