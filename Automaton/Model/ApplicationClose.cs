using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Automaton.Model
{
    class ApplicationClose
    {
        public static void Cleanup()
        {
            var processes = Process.GetProcesses().ToList();

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var sevenzipLocation = Path.Combine(baseDirectory, "7za.exe");
            var tempDirectory = Path.Combine(baseDirectory, "temp");

            if (File.Exists(sevenzipLocation))
            {
                var targetProcess = Process.GetProcessesByName("7za").First();

                if (targetProcess != null)
                {
                    targetProcess.Kill();
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
                streamWriter.WriteLine($"rmdir /s /q \"{tempDirectory}\"");
                streamWriter.WriteLine($"del /f \"{lastWordPath}\"");
            }

            Process.Start(lastWord);

        }
    }
}
