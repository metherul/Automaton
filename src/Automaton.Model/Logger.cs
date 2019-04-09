using Autofac;
using Automaton.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Alphaleonis.Win32.Filesystem;
using System.Security.Principal;

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
            _logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", $"Automaton_{Assembly.GetEntryAssembly().GetName().Version}_{string.Join("-", DateTime.Now.ToString().Split(Path.GetInvalidFileNameChars()))}.log");

            Task.Factory.StartNew(LoggerController);

            if (File.Exists(_logPath))
            {
                File.Delete(_logPath);
            }

            var platformType = Environment.Is64BitOperatingSystem ? "x64" : "x86";
            var headerString = $"Automaton/{Assembly.GetEntryAssembly().GetName().Version} ({Environment.OSVersion.VersionString}; {platformType}) {RuntimeInformation.FrameworkDescription}";

            WriteLine($"{DateTime.Now} {headerString}");
            WriteLine($"{DateTime.Now} Is admin: {new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator)}");
            WriteLine($"{DateTime.Now} Automaton Start");

            AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
            {
                WriteLine($"{eventArgs.Exception.StackTrace} {eventArgs.Exception.Message}");

                if (eventArgs.Exception.Message.Contains("materialDesign"))
                {
                    return;
                }

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
                lock (_logQueue)
                {
                    if (_logQueue.Any())
                    {
                        if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs")))
                        {
                            Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"));
                        }

                        File.AppendAllText(_logPath, _logQueue.Dequeue());
                    }
                }

                Thread.Sleep(10);
            }
        }
    }
}
