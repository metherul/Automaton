using System.Runtime.CompilerServices;

namespace Automaton.Model.Interfaces
{
    public interface ILogger : IService
    {
        void WriteLine(string message, [CallerMemberName] string callerName = "");
    }
}