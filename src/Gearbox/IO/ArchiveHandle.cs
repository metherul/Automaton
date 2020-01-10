using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Gearbox.IO
{
    public class ArchiveHandle
    {
        private readonly string _archivePath;

        public ArchiveHandle(string archivePath)
        {
            _archivePath = archivePath;
        }

        public async Task Extract(string extractDir)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            using var process = new Process();
            var procArguments = $"x \"{_archivePath}\" -o\"{extractDir}\" -y";
            var processStartInfo = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z.exe"),
                Arguments = procArguments,
                RedirectStandardOutput = true
            };

            process.StartInfo = processStartInfo;
            process.Start();

            await Task.Run(() => process.WaitForExit());

            stopwatch.Stop();
            var time = stopwatch.ElapsedMilliseconds;
        }
    }
}
