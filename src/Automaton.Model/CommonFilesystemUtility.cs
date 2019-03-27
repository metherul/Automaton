using Automaton.Model.Interfaces;
using System.Diagnostics;

namespace Automaton.Model
{
    public class CommonFilesystemUtility : ICommonFilesystemUtility
    {
        public void DeleteDirectory(string path)
        {
            var process = new Process();
            var startInfo = new ProcessStartInfo()
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                FileName = "cmd.exe",
                Arguments = $"/C rd /q /s \"{path}\""
            };

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }
    }
}
