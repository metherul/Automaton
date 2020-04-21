using System.Diagnostics;
using System.Threading.Tasks;

namespace Gearbox.Scripts
{
    internal class PowershellScript : IRunnableScript
    {
        private readonly string _scriptPath;

        public PowershellScript(string scriptPath) => _scriptPath = scriptPath;

        public async Task Run(string args = "")
        {
            var startInfo = new ProcessStartInfo()
            {
                FileName = _scriptPath,
                Arguments = args, 
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            var process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            await Task.Run(() => process.WaitForExit());
        }
    }
}
