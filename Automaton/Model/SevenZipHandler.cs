using SevenZipExtractor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace Automaton.Model
{
    class SevenZipHandler : IDisposable
    {
        private string ExecutablePath;
        private string ArchivePath;
        private string DLLPath;
        private string TempPath;
        private string ExtractionPath;
        
        public SevenZipHandler(string archivePath)
        {
            ExecutablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"7za.exe");
            ArchivePath = archivePath;
            DLLPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z.dll");
            TempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp");
            ExtractionPath = Path.Combine(TempPath, Path.GetFileNameWithoutExtension(archivePath));

            if (Directory.Exists(TempPath))
            {
                var directoryInfo = new DirectoryInfo(TempPath);
                directoryInfo.Delete(true);
            }

            ExtractArchive();
        }

        public void Install(string source, string installationPath, string target)
        {
            var currentTime = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            var sourcePath = Path.Combine(ExtractionPath, source);
            var targetPath = Path.Combine(installationPath, $"{PackHandler.FinalModPack.PackName} ({currentTime})", target);

            // Check if the sourceFile is a directory
            if (Path.GetExtension(sourcePath) == string.Empty)
            {
                // Grab a list of all of its contents
                var subDirectories = Directory.GetDirectories(sourcePath).ToList();
                var subFiles = Directory.GetFiles(sourcePath).ToList();

                var rootSubDirectories = subDirectories.Select(x => x.Replace($"{ExtractionPath}\\{source}\\", ""));
                var rootSubFiles = subFiles.Select(x => x.Replace($"{ExtractionPath}\\{source}\\", ""));

                foreach (var directory in subDirectories)
                {
                    var directoryName = new DirectoryInfo(directory).Name;
                    var directoryTarget = Path.Combine(targetPath, directoryName);

                    CopyFile(directory, directoryTarget, true);
                }

                foreach (var file in subFiles)
                {
                    var fileName = new FileInfo(file).Name;
                    var fileTarget = Path.Combine(targetPath, fileName);

                    CopyFile(file, fileTarget);
                }
            }

            // It's a file
            else
            {
                CopyFile(sourcePath, targetPath);
            }
        }

        private void ExtractArchive()
        {
            if (!Directory.Exists(TempPath))
            {
                Directory.CreateDirectory(TempPath);
            }

            var processStartInfo = new ProcessStartInfo()
            {
                FileName = ExecutablePath,
                Arguments = $"x \"{ArchivePath}\" -o\"{ExtractionPath}\" -y",
                RedirectStandardError = false,
                RedirectStandardOutput = false,
                UseShellExecute = true,
                CreateNoWindow = false
            };

            using (var process = new Process())
            {
                process.StartInfo = processStartInfo;
                process.Start();

                process.WaitForExit();
            }
        }

        private void CopyFile(string source, string target, bool isDirectory = false)
        {
            if (isDirectory)
            {
                Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(source, target);
            }

            else if (!isDirectory)
            {
                var parentTargetDirectory = new FileInfo(target).DirectoryName;

                if (!Directory.Exists(parentTargetDirectory))
                {
                    Directory.CreateDirectory(parentTargetDirectory);
                }

                File.Copy(source, target);
            }
        }

        public void Dispose()
        {
            // Delete the temp directory
            //Directory.Delete(TempPath, true);
        }
    }
}
