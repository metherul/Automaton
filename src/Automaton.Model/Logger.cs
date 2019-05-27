using System.Runtime.CompilerServices;
using Automaton.Model.Interfaces;

namespace Automaton.Model
{
    public class Logger : ILogger
    {
        public void Write(string message, [CallerMemberName] string callerName = "")
        {
            throw new System.NotImplementedException();
        }

        public void Write(string message, bool writeLine, [CallerMemberName] string callerName = "")
        {
            throw new System.NotImplementedException();
        }
    }
}
