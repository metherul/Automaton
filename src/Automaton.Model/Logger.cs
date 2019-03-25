using Automaton.Model.Interfaces;
using Sentry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Automaton.Model
{
    public class Logger : ILogger
    {
        private readonly string _logPath;
        private Queue<string> _logQueue = new Queue<string>();

        public Logger()
        {
            _logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");

            Task.Factory.StartNew(LoggerController);

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
            _logQueue.Enqueue($"[{callerName}] {message}\n");
        }

        private void LoggerController()
        {
            while (true)
            {
                if (_logQueue.Any())
                {
                    File.AppendAllText(_logPath, _logQueue.Dequeue());
                }

                Thread.Sleep(10);
            }
        }
    }
}
