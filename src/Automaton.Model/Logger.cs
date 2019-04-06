using Autofac;
using Automaton.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Automaton.Model
{
    public class Logger : ILogger
    {
        public EventHandler<FirstChanceExceptionEventArgs> CapturedError { get; set; }
        public EventHandler<string> CapturedLog { get; set; }

        private readonly string _logPath;
        private Queue<string> _logQueue = new Queue<string>();

        public Logger(IComponentContext components)
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

                CapturedError.Invoke(this, eventArgs);
            };
        }

        public void WriteLine(string message, bool requiresDisplay = false, [CallerMemberName] string callerName = "")
        {
            _logQueue.Enqueue($"[{DateTime.Now.TimeOfDay}] [{callerName}] {message}\n");

            if (requiresDisplay)
            {
                CapturedLog.Invoke(this, message);
            }
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
