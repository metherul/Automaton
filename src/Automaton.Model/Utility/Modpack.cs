using Automaton.Model.Errors;
using Automaton.Model.Extensions;
using Automaton.Model.Instance;
using Automaton.Model.ModpackBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.Devices;

namespace Automaton.Model.Utility
{
    public class Modpack
    {
        public delegate void ModpackLoaded();

        public static event ModpackLoaded ModpackLoadedEvent;

        /// <summary>
        /// Extracts and loads an archived modpack into the model
        /// </summary>
        /// <param name="modpackPath"></param>
        public static void LoadModpack(string modpackPath)
        {
            var modpackExtractionPath = string.Empty;
            var modpackHeader = new Header();
            var modpackMods = new List<Mod>();

            // Extract the modpack archive out to a temp folder
            using (var extractionHandler = new ArchiveExtractor(modpackPath))
            {
                extractionHandler.ExtractModpack();
            }

            modpackExtractionPath = AutomatonInstance.ModpackExtractionLocation;

            // Load the modpack header
            var modpackHeaderPath = Path.Combine(modpackExtractionPath, $"modpack.json");

            if (!File.Exists(modpackHeaderPath))
            {
                GenericErrorHandler.Throw(GenericErrorType.ModpackStructure, "Valid modpack.json was not found.", new StackTrace());
                return;
            }

            // Load modpack header into Instance
            modpackHeader = Json.TryDeserializeJson<Header>(File.ReadAllText(modpackHeaderPath), out string parseError);

            // The json string was not parsed correctly, throw error
            if (parseError != string.Empty)
            {
                GenericErrorHandler.Throw(GenericErrorType.JSONParse, parseError, new StackTrace());
                return;
            }

            // Set global instances, these will update viewmodels automatically via the message service
            Instance.AutomatonInstance.ModpackHeader = modpackHeader;
            Instance.AutomatonInstance.ModpackMods = LoadModInstallParameters(modpackHeader, modpackExtractionPath);

            ModpackLoadedEvent();
        }

        /// <summary>
        /// Updates the target mod's archive path
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="archivePath"></param>
        public static void UpdateModArchivePaths(Mod mod, string archivePath)
        {
            Instance.AutomatonInstance.ModpackMods.Where(x => x == mod).First().FilePath = archivePath;

            var test = Instance.AutomatonInstance.ModpackMods;
        }

        /// <summary>
        /// Loads and returns list of Mod objects
        /// </summary>
        /// <param name="modpackHeader"></param>
        /// <param name="modpackExtractionPath"></param>
        /// <returns></returns>
        private static List<Mod> LoadModInstallParameters(Header modpackHeader, string modpackExtractionPath)
        {
            var modpackMods = new List<Mod>();

            // Detect for mod install directories outlined by ModInstallFolders
            var modInstallFolders = modpackHeader.ModInstallFolders
                .Select(x => System.IO.Path.Combine(modpackExtractionPath, x).StandardizePathSeparators());

            var existingModInstallFolders = modInstallFolders
                .Where(x => Directory.Exists(x) && Directory.GetFiles(x, $"*.json").Any());

            // Check for any valid values
            if (!existingModInstallFolders.ContainsAny())
            {
                GenericErrorHandler.Throw(GenericErrorType.ModpackStructure, "mod_install_folders not found in modpack.", new StackTrace());
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
                        GenericErrorHandler.Throw(GenericErrorType.JSONParse, parseError, new StackTrace());

                        return null;
                    }

                    modpackMods.Add(modObject);
                }
            }

            return modpackMods;
        }

        public static void InstallModpack(IProgress<InstallModpackProgress> progress)
        {
            var progressObject = new InstallModpackProgress();

            // If Mod Organizer is set to be used, locate it and install
            if (AutomatonInstance.ModpackHeader.InstallModOrganizer)
            {

            }

            var mods = AutomatonInstance.ModpackMods;
            var extractionDirectory = AutomatonInstance.ModpackExtractionLocation;

            foreach (var mod in mods)
            {
                progressObject.UpdateString = $"{mod.ModName} | ({mods.IndexOf(mod) + 1}, {mods.Count})";
                progress.Report(progressObject);

                // Extract the archive into the extract folder
                var archivePath = mod.FilePath;
                var archiveExtractor = new ArchiveExtractor(archivePath);

                var modExtractionPath = Path.Combine(extractionDirectory, mod.ModName);
                string modInstallPath = Path.Combine(AutomatonInstance.InstallLocation, mod.ModName);

                // Extract the archive into the target path
                var extractionResponse = archiveExtractor.ExtractArchive(modExtractionPath);

                // Something broke during the extraction process, pass message back to user and attempt to solve
                if (!extractionResponse)
                {

                }
                
                // Catch non-existent install parameters. In this instance, move all files / directories.
                if (!mod.InstallationParameters.ContainsAny())
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
                    var targetPath = Path.Combine(AutomatonInstance.InstallLocation, installParameter.TargetLocation.StandardizePathSeparators());

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
                archiveExtractor.Dispose();
            }
        }

        private static void CopyDirectory(string sourcePath, string destinationPath)
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