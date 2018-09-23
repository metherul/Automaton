using Automaton.Model.Instance;
using System;
using System.Diagnostics;
using System.IO;

namespace Automaton.Model.Utility
{
    public class ArchiveExtractor : IDisposable
    {
        private readonly string TempDirectory = Path.Combine(Path.GetTempPath(), "Automaton");
        private readonly string MetaDirectory = AppDomain.CurrentDomain.BaseDirectory;

        private readonly string SevenZipPath;
        private readonly string SevenZipDLLPath;

        public string ExtractionPath;
        public string ArchivePath;

        public ArchiveExtractor(string archivePath)
        {
            SevenZipPath = Path.Combine(TempDirectory, "7z.exe");
            SevenZipDLLPath = Path.Combine(TempDirectory, "7z.dll");
            ExtractionPath = Path.Combine(TempDirectory, "extract");
            ArchivePath = archivePath;

            ExtractSevenzipBinaries();
        }

        public bool ExtractArchive(string extractionPath)
        {
            return Extract(extractionPath);
        }

        public bool ExtractModpack()
        {
            return Extract(AutomatonInstance.ModpackExtractionLocation);
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
                UseShellExecute = true
            };

            var process = new Process
            {
                StartInfo = processInfo
            };

            process.Start();
            process.WaitForExit();

            return true;
        }

        /// <summary>
        /// Extracts the sevenzip binaries from the application's resources into the temp directory.
        /// </summary>
        private void ExtractSevenzipBinaries()
        {
            if (!File.Exists(SevenZipPath))
            {
                Resources.WriteResourceToFile("Automaton.Content.Resources.7z.exe", SevenZipPath);
            }

            if (!File.Exists(SevenZipDLLPath))
            {
                Resources.WriteResourceToFile("Automaton.Content.Resources.7z.dll", SevenZipDLLPath);
            }
        }
    }
}