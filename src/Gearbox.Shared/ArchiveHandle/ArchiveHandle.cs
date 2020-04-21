using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Gearbox.Shared.ArchiveHandle
{
    public class ArchiveHandle
    {
        private readonly string _archivePath;

        public delegate void FileExtracted(string path);
        public event FileExtracted FileExtractedEvent;

        public ArchiveHandle(string archivePath)
        {
            _archivePath = archivePath;
        }

        public async Task Extract(string extractDir)
        {
            using var process = new Process();
            var procArguments = $"x \"{_archivePath}\" -o\"{extractDir}\" -y -bsp1 -bb2 -sccUTF-8 -mmt=on";
            var processStartInfo = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = Path.Combine(Directory.GetCurrentDirectory(), "7z.exe"),
                Arguments = procArguments,
                RedirectStandardOutput = true,
                StandardOutputEncoding = Encoding.UTF8
            };

            process.StartInfo = processStartInfo;
            process.Start();

            var last = "";
            while (!process.StandardOutput.EndOfStream)
            {
                var line = await process.StandardOutput.ReadLineAsync();

                // If the line contains no data, ignore it.
                if (string.IsNullOrWhiteSpace(line) || !line.StartsWith("- "))
                {
                    continue;
                }

                // Grab the relative path, removing: '- ' from the line.
                var path = Path.Combine(extractDir, line[2..]);

                // If the path is actually a directory, ignore it.
                if (Directory.Exists(path))
                {
                    continue;
                }

                // To prevent issues with file ownership timings, we only return the *last* file extracted.
                if (last == string.Empty)
                {
                    last = path;
                    continue;
                }

                // Pass the path back to the event handler.
                FileExtractedEvent.Invoke(last);
                last = path;
            }

            // Take care of the very last path.
            FileExtractedEvent.Invoke(last);

            await Task.Run(() => process.WaitForExit());
            process.Close();
        }
    }
}
