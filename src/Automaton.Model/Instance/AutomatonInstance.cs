using Automaton.Model.ModpackBase;
using System.Collections.Generic;
using System.IO;

namespace Automaton.Model.Instance
{
    public class AutomatonInstance
    {
        public static Header ModpackHeader { get; set; } = new Header();
        public static List<Mod> ModpackMods { get; set; } = new List<Mod>();

        public static string SourceLocation { get; set; }

        public static string InstallLocation { get; set; }
        public static string ModOrganizerInstallLocation { get; set; }

        public static string TempDirectory { get; set; }
        public static string ExtractionLocation { get; set; } 
        public static string ModpackExtractionLocation { get; set; }

        public static string NexusHandlerRegistryValue { get; set; } = "\"C:\\Programming\\C#\\NXMWorker\\src\\NXMWorker\\bin\\Debug\\NXMWorker.exe\" \"%1\"";
        public static string PreviousRegistryValue { get; set; }

        public static void InitializeInstance()
        {
            TempDirectory = Path.GetTempPath();
            ExtractionLocation = Path.Combine(TempDirectory, "Automaton", "extract");
            ModpackExtractionLocation = Path.Combine(TempDirectory, "Automaton", "modpack");
        }

        #region Modification Methods

        /// <summary>
        /// Adds a new ModInstallFolder to the instance
        /// </summary>
        /// <param name="path"></param>
        public static void AddModInstallFolder(string path)
        {
            var tempModpackHeader = AutomatonInstance.ModpackHeader;
            tempModpackHeader.ModInstallFolders.Add(path);

            AutomatonInstance.ModpackHeader = tempModpackHeader;
        }

        /// <summary>
        /// Removes a ModInstallFolder from the instance
        /// </summary>
        /// <param name="path"></param>
        public static void RemoveModInstallFolder(string path)
        {
            var tempModpackHeader = AutomatonInstance.ModpackHeader;
            tempModpackHeader.ModInstallFolders.RemoveAll(x => x == path);

            AutomatonInstance.ModpackHeader = tempModpackHeader;
        }

        #endregion Modification Methods
    }
}