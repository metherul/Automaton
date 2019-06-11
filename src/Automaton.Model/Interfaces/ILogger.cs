using System.Runtime.CompilerServices;

namespace Automaton.Model.Interfaces
{
    public interface ILogger : IModel
    {
        void Write(string message, [CallerMemberName] string callerName = "");
        void Write(string message, bool writeLine, [CallerMemberName] string callerName = "");
    }
}
