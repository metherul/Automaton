using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Automaton.Model.Handles
{
    public class ArchiveHandler : IDisposable
    {
        private readonly string TempDirectory = Path.Combine(Path.GetTempPath(), "Automaton");
        private readonly string MetaDirectory = AppDomain.CurrentDomain.BaseDirectory;

        private readonly string SevenZipPath;
        private readonly string SevenZipDLLPath;

        public string ExtractionPath;
        public string ModpackExtractionPath;
        public string ArchivePath;

        public ArchiveHandler(string archivePath)
        {
            SevenZipPath = Path.Combine(TempDirectory, "7z.exe");
            SevenZipDLLPath = Path.Combine(TempDirectory, "7z.dll");
            ExtractionPath = Path.Combine(TempDirectory, "extract");
            ModpackExtractionPath = Path.Combine(TempDirectory, "modpack_temp");
            ArchivePath = archivePath;

            ExtractSevenzipBinaries();
        }

        public bool ExtractArchive()
        {
            return false;
        }

        public bool ExtractModpack()
        {
            return Extract(ModpackExtractionPath);
        }

        public void Dispose()
        {
            if (Directory.Exists(ExtractionPath))
            {
                Directory.Delete(ExtractionPath, true);
            }
        }

        private bool Extract(string extractionPath)
        {
            if (Directory.Exists(extractionPath))
            {
                Directory.Delete(extractionPath, true);
            }

            var processInfo = new ProcessStartInfo()
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = SevenZipPath,
                Arguments = $"x \"{ArchivePath}\" -o\"{extractionPath}\" -y",
                UseShellExecute = false
            };

            var process = new Process
            {
                StartInfo = processInfo
            };

            process.Start();
            process.WaitForExit();

            return true;
        }

        private void ExtractSevenzipBinaries()
        {
            var test = Assembly.GetEntryAssembly().GetManifestResourceNames();

            if (!File.Exists(SevenZipPath))
            {
                WriteResourceToFile("Automaton.Content.Resources.7z.exe", SevenZipPath);
            }

            if (!File.Exists(SevenZipDLLPath))
            {
                WriteResourceToFile("Automaton.Content.Resources.7z.dll", SevenZipDLLPath);
            }
        }

        private void WriteResourceToFile(string resourceName, string fileName)
        {
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            }

            using (var resource = Assembly.GetEntryAssembly().GetManifestResourceStream(resourceName))
            {
                using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    resource.CopyTo(file);
                }
            }
        }
    }
}