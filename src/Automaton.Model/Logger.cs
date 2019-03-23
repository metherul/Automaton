using Automaton.Model.Interfaces;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Automaton.Model
{
    public class Logger : ILogger
    {
        private readonly string _logPath;

        public Logger()
        {
            _logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
            
            if (File.Exists(_logPath))
            {
                File.Delete(_logPath);
            }

            WriteLine($"{DateTime.Now} Automaton Start");

            AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
            {
                WriteLine($"{eventArgs.Exception.StackTrace} {eventArgs.Exception.Message}");
            };
        }

        public void WriteLine(string message, [CallerMemberName] string callerName = "")
        {
            File.AppendAllText(_logPath, $"[{callerName}] {message}\n");
        }
    }
}
