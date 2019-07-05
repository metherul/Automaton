using System.Threading.Tasks;
using System.Windows.Input;

namespace Automaton.ViewModel.Utilities.Interfaces
{
    public interface IGenericAsyncCommand<T> : ICommand
    {
        Task ExecuteAsync(T parameter);
        bool CanExecute(T parameter);
    }
}