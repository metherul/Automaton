using System;
using System.Diagnostics;
using System.IO;
using Automaton.Model.Instance.Interfaces;
using Automaton.Model.Utility.Interfaces;

namespace Automaton.Model.Utility
{
    public class ArchiveExtractor : IArchiveExtractor, IDisposable
    {
        private readonly IAutomatonInstance _automatonInstance;

        private readonly string _tempDirectory = Path.Combine(Path.GetTempPath(), "Automaton");
        private readonly string _metaDirectory = AppDomain.CurrentDomain.BaseDirectory;

        private string _sevenZipPath;
        private string _sevenZipDllPath;

        public string ExtractionPath;
        public string ArchivePath;

        public ArchiveExtractor(IAutomatonInstance automatonInstance)
        {
            _automatonInstance = automatonInstance;
        }

        public void TargetArchive(string archivePath)
        {
            _sevenZipPath = Path.Combine(_tempDirectory, "7z.exe");
            _sevenZipDllPath = Path.Combine(_tempDirectory, "7z.dll");
            ExtractionPath = Path.Combine(_tempDirectory, "extract");
            ArchivePath = archivePath;

            ExtractSevenzipBinaries();
        }

        public bool ExtractArchive(string extractionPath)
        {
            return Extract(extractionPath);
        }

        public bool ExtractModpack()
        {
            return Extract(_automatonInstance.ModpackExtractionLocation);
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
                FileName = _sevenZipPath,
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
            if (!File.Exists(_sevenZipPath))
            {
                Resources.WriteResourceToFile("Automaton.Content.Resources.7z.exe", _sevenZipPath);
            }

            if (!File.Exists(_sevenZipDllPath))
            {
                Resources.WriteResourceToFile("Automaton.Content.Resources.7z.dll", _sevenZipDllPath);
            }
        }
    }
}