using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Automaton.Model.Errors;
using Automaton.Model.Extensions;
using Automaton.Model.Handles;
using Automaton.Model.Instances;

namespace Automaton.Model.Modpack
{
    public class ModpackUtilities
    {
        /// <summary>
        /// Extracts and loads an archived modpack into the model
        /// </summary>
        /// <param name="modpackPath"></param>
        public static void LoadModPack(string modpackPath)
        {
            var modpackExtractionPath = string.Empty;
            var modpackHeader = new ModpackHeader();
            var modpackMods = new List<Mod>();

            // Extract the modpack archive out to a temp folder
            using (var extractionHandler = new ArchiveHandler(modpackPath))
            {
                extractionHandler.ExtractModpack();
                modpackExtractionPath = extractionHandler.ModpackExtractionPath;
            }

            ModpackInstance.ModpackExtractionLocation = modpackExtractionPath;

            // Load the modpack header
            var modpackHeaderPath = Path.Combine(modpackExtractionPath, $"modpack.json");

            if (!File.Exists(modpackHeaderPath))
            {
                GenericErrorHandler.Throw(GenericErrorType.ModpackStructure, "Valid modpack.json was not found.", new StackTrace());
                return;
            }

            // Load modpack header into Instance
            modpackHeader = JSONHandler.TryDeserializeJson<ModpackHeader>(File.ReadAllText(modpackHeaderPath), out string parseError);

            // The json string was not parsed correctly, throw error
            if (parseError != string.Empty)
            {
                GenericErrorHandler.Throw(GenericErrorType.JSONParse, parseError, new StackTrace());
                return;
            }

            // Set global instances, these will update viewmodels automatically via the message service
            ModpackInstance.ModpackHeader = modpackHeader;
            ModpackInstance.ModpackMods = LoadModInstallParameters(modpackHeader, modpackExtractionPath);
        }

        /// <summary>
        /// Loads and returns list of Mod objects
        /// </summary>
        /// <param name="modpackHeader"></param>
        /// <param name="modpackExtractionPath"></param>
        /// <returns></returns>
        private static List<Mod> LoadModInstallParameters(ModpackHeader modpackHeader, string modpackExtractionPath)
        {
            var modpackMods = new List<Mod>();

            // Detect for mod install directories outlined by ModInstallFolders
            var modInstallFolders = modpackHeader.ModInstallFolders
                .Select(x => Path.Combine(modpackExtractionPath, x).StandardizePathSeparators());

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
                    var modObject = JSONHandler.TryDeserializeJson<Mod>(File.ReadAllText(modFile), out string parseError);

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

        /// <summary>
        /// Updates <see cref="ModpackHeader"/> mod list value with correct built paths
        /// </summary>
        public static void UpdateModInstallParameters()
        {
            var modpackHeader = ModpackInstance.ModpackHeader;
            var modpackExtractionPath = ModpackInstance.ModpackExtractionLocation;

            ModpackInstance.ModpackMods = LoadModInstallParameters(modpackHeader, modpackExtractionPath);
        }

        /// <summary>
        /// Returns a list of type <see cref="Mod"/> which contains mods that were not able to be found in the standard source directory
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <returns></returns>
        public static List<Mod> GetModsWithMissingArchives(string sourceDirectory)
        {
            var modInstallParameters = ModpackInstance.ModpackMods;

            foreach (var mod in modInstallParameters)
            {
            }

            return null;
        }

        /// <summary>
        /// Returns true/false on whether paths outlined by <see cref="Mod.ModArchivePath"/> exist
        /// Checks filesize and MD5Sum (on edge cases) to confirm that the archive is correct
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <returns></returns>
        public static bool ValidModArchiveLocations(string sourceDirectory)
        {
            return false;
        }

        /// <summary>
        /// Determines is parameterized mod archive path exists and contains correct data
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public static bool DoesModArchivePathExist(Mod mod)
        {
            return false;
        }

        /// <summary>
        /// Builds initial modpack mod source archive paths <see cref="Mod.ModArchivePath"/>
        /// </summary>
        /// <returns></returns>
        public static void BuildModArchivePaths()
        {
        }

        /// <summary>
        /// Patches updated source path into modpack at specified <see cref="Mod"/> value with path
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static void PatchModArchivePath(Mod mod, string path)
        {
        }
    }
}