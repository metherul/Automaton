using Automaton.Model.Extensions;
using Automaton.Model.ModpackBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Automaton.Model.Instance.Interfaces;
using Automaton.Model.Utility.Interfaces;

namespace Automaton.Model.Utility
{
    public class ModpackUtilities : IModpackUtilties
    {
        private readonly IAutomatonInstance _automatonInstance;
        private readonly IArchiveExtractor _archiveExtractor;

        public ModpackUtilities(IAutomatonInstance automatonInstance, IArchiveExtractor archiveExtractor)
        {
            _automatonInstance = automatonInstance;
        }

        /// <summary>
        /// Extracts and loads an archived modpack into the model
        /// </summary>
        /// <param name="modpackPath"></param>
        public bool LoadModpack(string modpackPath)
        {
            _archiveExtractor.TargetArchive(modpackPath);
            _archiveExtractor.ExtractModpack();

            // Load the modpack header
            var modpackHeaderPath = Path.Combine(_automatonInstance.ModpackExtractionLocation, $"modpack.json");

            if (!File.Exists(modpackHeaderPath))
            {
                return false;
            }

            // Load modpack header into Instance
            var modpackHeader = Json.TryDeserializeJson<Header>(File.ReadAllText(modpackHeaderPath), out string parseError);

            // The json string was not parsed correctly, throw error
            if (parseError != string.Empty)
            {
                return false;
            }

            // Set global instances, these will update viewmodels automatically via the message service
            _automatonInstance.ModpackHeader = modpackHeader;
            _automatonInstance.ModpackMods = LoadModInstallParameters(modpackHeader, _automatonInstance.ModpackExtractionLocation);

            return true;
        }

        /// <summary>
        /// Updates the target mod's archive path
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="archivePath"></param>
        public void UpdateModArchivePaths(Mod mod, string archivePath)
        {
            _automatonInstance.ModpackMods.First(x => x == mod).FilePath = archivePath;
        }

        /// <summary>
        /// Loads and returns list of Mod objects
        /// </summary>
        /// <param name="modpackHeader"></param>
        /// <param name="modpackExtractionPath"></param>
        /// <returns></returns>
        private List<Mod> LoadModInstallParameters(Header modpackHeader, string modpackExtractionPath)
        {
            var modpackMods = new List<Mod>();

            // Detect for mod install directories outlined by ModInstallFolders
            var modInstallFolders = modpackHeader.ModInstallFolders
                .Select(x => System.IO.Path.Combine(modpackExtractionPath, x).StandardizePathSeparators());

            var existingModInstallFolders = modInstallFolders
                .Where(x => Directory.Exists(x) && Directory.GetFiles(x, $"*.json").Any());

            // Check for any valid values
            if (!existingModInstallFolders.NullAndAny())
            {
                return null;
            }

            // Out to log or error handler, not a breaking issue, but may cause installation issues
            if (existingModInstallFolders.Count() != modInstallFolders.Count())
            {
                // TODO
            }

            // Parse existingModInstallFolders and return any valid mod structures
            foreach (var folder in existingModInstallFolders)
            {
                var modFiles = Directory.GetFiles(folder, $"*.json");

                foreach (var modFile in modFiles)
                {
                    var modObject = Json.TryDeserializeJson<Mod>(File.ReadAllText(modFile), out string parseError);

                    modObject.ModInstallParameterPath = modFile;

                    if (parseError != string.Empty)
                    {
                        return null;
                    }

                    modpackMods.Add(modObject);
                }
            }

            return modpackMods;
        }

        public void InstallModpack(IProgress<InstallModpackProgress> progress)
        {
            var progressObject = new InstallModpackProgress();

            // If Mod Organizer is set to be used, locate it and install
            if (_automatonInstance.ModpackHeader.InstallModOrganizer)
            {
            }

            var mods = _automatonInstance.ModpackMods;
            var extractionDirectory = _automatonInstance.ModpackExtractionLocation;

            foreach (var mod in mods)
            {
                progressObject.UpdateString = $"{mod.ModName} | ({mods.IndexOf(mod) + 1}, {mods.Count})";
                progress.Report(progressObject);

                // Extract the archive into the extract folder
                var archivePath = mod.FilePath;

                var modExtractionPath = Path.Combine(extractionDirectory, mod.ModName);
                string modInstallPath = Path.Combine(_automatonInstance.InstallLocation, mod.ModName);

                // Extract the archive into the target path
                _archiveExtractor.TargetArchive(archivePath);
                var extractionResponse = _archiveExtractor.ExtractArchive(modExtractionPath);

                // Something broke during the extraction process, pass message back to user and attempt to solve
                if (!extractionResponse)
                {

                }
                
                // Catch non-existent install parameters. In this instance, move all files / directories.
                if (!mod.InstallationParameters.NullAndAny())
                {
                    if (Directory.Exists(modInstallPath))
                    {
                        Directory.Delete(modInstallPath, true);
                    }

                    Directory.Move(modExtractionPath, modInstallPath);

                    continue;
                }

                foreach (var installParameter in mod.InstallationParameters)
                {
                    var sourcePath = Path.Combine(modExtractionPath, installParameter.SourceLocation.StandardizePathSeparators());
                    var targetPath = Path.Combine(_automatonInstance.InstallLocation, installParameter.TargetLocation.StandardizePathSeparators());

                    // Copy/move operation if the source is a directory
                    if (Directory.Exists(sourcePath))
                    {
                        CopyDirectory(sourcePath, targetPath);
                    }

                    // Copy/move directory if the source is a file
                    if (File.Exists(sourcePath))
                    {
                        File.Copy(sourcePath, targetPath);
                    }
                }

                // Delete the extracted files
                _archiveExtractor.Dispose();
            }
        }

        private void CopyDirectory(string sourcePath, string destinationPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*",
                SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*",
                SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
            }
        }
    }

    public class InstallModpackProgress
    {
        public string UpdateString;
    }
}