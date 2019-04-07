using Automaton.Model.Interfaces;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Automaton.Model.Interfaces
{
    public interface ILogger : IService
    {
        EventHandler<FirstChanceExceptionEventArgs> CapturedError { get; set; }
        EventHandler<string> CapturedLog { get; set; }

        void WriteLine(string message, bool requiresDisplay = false, [CallerMemberName] string callerName = "");
    }
}