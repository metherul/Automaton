using Automaton.Handles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Automaton.Model
{
    public class SevenZipHandler : IDisposable
    {
        private static string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static string TempDirectory = Path.Combine(BaseDirectory, "temp");
        private static string ExePath = Path.Combine(BaseDirectory, "7z.exe");
        private static string DLLPath = Path.Combine(BaseDirectory, "7z.dll");

        private static string ExtractedFilePath;
        private static string InstallationLocation;

        public SevenZipHandler()
        {
            // Load the embedded resource into memory, and write to temporary file.
            var embeddedApplication = Properties.Resources._7z;
            var embeddedDLL = Properties.Resources._7zDLL;

            if (File.Exists(ExePath))
            {
                File.Delete(ExePath);
            }

            if (File.Exists(DLLPath))
            {
                File.Delete(DLLPath);
            }

            using (var fileStream = new FileStream(ExePath, FileMode.CreateNew))
            {
                fileStream.Write(embeddedApplication, 0, embeddedApplication.Length);
            }

            using (var fileStream = new FileStream(DLLPath, FileMode.CreateNew))
            {
                fileStream.Write(embeddedDLL, 0, embeddedDLL.Length);
            }

            InstallationLocation = PackHandler.InstallationLocation;
        }

        /// <summary>
        /// Will extract the archive to the temp directory and target it within the class.
        /// </summary>
        /// <param name="path">The path of the archive file (.7z, .zip, .rar)</param>
        public void ExtractArchive(string path)
        {
            if (Directory.Exists(ExtractedFilePath))
            {
                DeleteDirectory(ExtractedFilePath);
            }

            var extractedPath = Path.Combine(TempDirectory, Path.GetFileNameWithoutExtension(path));

            if (Directory.Exists(extractedPath))
            {
                DeleteDirectory(extractedPath);
            }

            Directory.CreateDirectory(extractedPath);

            // Spool up a process to extract the file using 7za.exe
            var processStartInfo = new ProcessStartInfo()
            {
                FileName = ExePath,
                Arguments = $"x \"{path}\" -o\"{extractedPath}\" -y",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process())
            {
                process.StartInfo = processStartInfo;
                process.Start();

                // Note that this method must be async to prevent blocking UI calls
                process.WaitForExit();
            }

            ExtractedFilePath = extractedPath;
        }

        /// <summary>
        /// Deletes the extracted folder from the temp directory.
        /// </summary>
        /// <param name="path"></param>
        public void DeleteExtractedFiles(string path)
        {
            var extractedPath = Path.Combine(TempDirectory, Path.GetFileNameWithoutExtension(path));

            if (Directory.Exists(extractedPath))
            {
                DeleteDirectory(extractedPath);
            }
        }

        /// <summary>
        /// Copies a file/directory from a location within the extracted archive to a target.
        /// </summary>
        /// <param name="source">The source location -- denoted in a locational offset within the archive.</param>
        /// <param name="target">The target location -- denoted in a location offset within the installation location.</param>
        public void Copy(Mod mod, Installation installation, string source, string target)
        {
            source = CleanupPath(source);
            target = CleanupPath(target);

            var realSourceLocation = Path.Combine(ExtractedFilePath, source);
            var realTargetLocation = Path.Combine(InstallationLocation, mod.ModName, target);

            var ignoresTarget = new List<string>();

            if (installation.Ignores != null && installation.Ignores.Any())
            {
                ignoresTarget = installation.Ignores.ToList()
                    .Select(x => Path.Combine(realTargetLocation, CleanupPath(x))).ToList();
            }

            if (!DoesPathExist(realSourceLocation))
            {
                throw new Exception($"Targeted source location does not exist: {realSourceLocation}");
            }

            if (IsPathDirectory(realSourceLocation))
            {
                // Files with the realSourceLocation removed.
                var sourceFiles = Directory.GetFileSystemEntries(realSourceLocation, "*.*", SearchOption.AllDirectories).ToList()
                    .Where(x => !IsPathDirectory(x)).ToList();

                var targetFiles = sourceFiles
                    .Select(x => new Uri(realSourceLocation).MakeRelativeUri(new Uri(x)).ToString())
                    .Select(x => x = Uri.UnescapeDataString(x).ToString())
                    .Select(x => Path.Combine(realTargetLocation, x)).ToList();

                foreach (var ignore in ignoresTarget)
                {
                    LoadingDialogHandle.UpdateDebugText($"Ignoring: \"{installation.Ignores[ignoresTarget.IndexOf(ignore)]}\"");

                    var index = targetFiles.IndexOf(ignore);
                    sourceFiles.RemoveAt(index);
                    targetFiles.Remove(ignore);
                }

                foreach (var file in targetFiles)
                {
                    var directory = new FileInfo(file).DirectoryName;
                    var matchingSourceFile = sourceFiles[targetFiles.IndexOf(file)];

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    File.Copy(matchingSourceFile, file, true);
                }
            }

            else
            {
                var directory = new FileInfo(realTargetLocation).DirectoryName;

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.Copy(realSourceLocation, realTargetLocation, true);
            }
        }

        /// <summary>
        /// Determines whether a specified path is a directory, or a file.
        /// </summary>
        /// <param name="path">A file or directory.</param>
        /// <returns></returns>
        private bool IsPathDirectory(string path)
        {
            var fileAttributes = File.GetAttributes(path);

            if (fileAttributes.HasFlag(FileAttributes.Directory))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether a specfied path exists on the drive.
        /// </summary>
        /// <param name="path">A file or directory.</param>
        /// <returns></returns>
        private bool DoesPathExist(string path)
        {
            if (Directory.Exists(path))
            {
                return true;
            }

            if (File.Exists(path))
            {
                return true;
            }

            return false;

        }

        /// <summary>
        /// Clean up the string paths so they don't break as much
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string CleanupPath(string path)
        {
            var isFile = Path.HasExtension(path);

            if (path.StartsWith("/"))
            {
                path = path.TrimStart('/');
            }

            if (!path.EndsWith("/") && !string.IsNullOrEmpty(path) && !isFile)
            {
                path += "/";
            }

            return path;
        }

        /// <summary>
        /// Will delete a directory, even if it is ReadOnly
        /// </summary>
        /// <param name="path"></param>
        private void DeleteDirectory(string directoryPath)
        {
            var rootInfo = new DirectoryInfo(directoryPath) { Attributes = FileAttributes.Normal };

            foreach (var fileInfo in rootInfo.GetFileSystemInfos())
            {
                fileInfo.Attributes = FileAttributes.Normal;
            }

            foreach (var subDirectory in Directory.GetDirectories(directoryPath, "*", SearchOption.AllDirectories))
            {
                var subInfo = new DirectoryInfo(subDirectory)
                {
                    Attributes = FileAttributes.Normal
                };

                foreach (var fileInfo in subInfo.GetFileSystemInfos())
                {
                    fileInfo.Attributes = FileAttributes.Normal;
                }
            }

            Directory.Delete(directoryPath, true);
        }

        public void Dispose()
        {
            if (File.Exists(ExePath))
            {
                File.Delete(ExePath);
            }

            if (Directory.Exists(TempDirectory))
            {
                Directory.Delete(TempDirectory, true);
            }
        }
    }
}
