using Alphaleonis.Win32.Filesystem;
using System;
using System.Diagnostics;
using System.Linq;

namespace Automaton.Model
{
    class ApplicationClose
    {
        public static void Cleanup()
        {
            var processes = Process.GetProcesses().ToList();

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var sevenzipLocation = Path.Combine(baseDirectory, "7z.exe");
            var sevenzipDLL = Path.Combine(baseDirectory, "7z.dll");
            var tempDirectory = Path.Combine(baseDirectory, "temp");

            if (File.Exists(sevenzipLocation))
            {
                var targetProcess = Process.GetProcessesByName("7z");

                if (targetProcess.Any())
                {
                    targetProcess.First().Kill();
                }
            }

            var lastWordPath = Path.Combine(baseDirectory, "mylastword.bat");
            var lastWord = new ProcessStartInfo()
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = lastWordPath
            };

            using (var streamWriter = File.CreateText(lastWordPath))
            {
                streamWriter.WriteLine($"ping -n 1 127.0.0.1 > nul");
                streamWriter.WriteLine($"del /f \"{sevenzipLocation}\"");
                streamWriter.WriteLine($"del /f \"{sevenzipDLL}\"");
                streamWriter.WriteLine($"rmdir /s /q \"{tempDirectory}\"");
                streamWriter.WriteLine($"del /f \"{lastWordPath}\"");
            }

            Process.Start(lastWord);

        }
    }
}
