using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Automaton.Model.Archive.Interfaces;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;

namespace Automaton.Model.Archive
{
    public class ArchiveContents : IArchiveContents
    {
        public List<IArchiveEntry> GetArchiveEntries(string archivePath)
        {
            var archive = ArchiveFactory.Open(archivePath);
            var test = archive.Entries.ToList();

            return archive.Entries.ToList();
        }

        public MemoryStream GetMemoryStreamFromEntry(Entry entry)
        {
            var memoryStream = new MemoryStream();

            return memoryStream;
        }

        public void ExtractToDirectory(string archivePath, string directoryPath)
        {
            var sevenZipExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z.exe");

            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }

            Directory.CreateDirectory(directoryPath);

            var process = new Process();
            var startInfo = new ProcessStartInfo()
            {
                FileName = sevenZipExe,
                Arguments = $"x \"{archivePath}\" -o\"{directoryPath}\""
            };

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }
    }
}
