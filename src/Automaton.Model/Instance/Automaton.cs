using System.Collections.Generic;
using Automaton.Model.ModpackBase;
using Automaton.Model.Utility;

namespace Automaton.Model.Instance
{
    public class Automaton
    {
        public static Header ModpackHeader { get; set; } = new Header();
        public static List<Mod> ModpackMods { get; set; } = new List<Mod>();

        public static string MoInstallLocation { get; set; }
        public static string SourceLocation { get; set; }
        public static string ModpackExtractionLocation { get; set; }

        public string NexusHandlerRegistryValue { get; set; }
        public static string PreviousRegistryValue { get; set; }

        #region Modification Methods

        /// <summary>
        /// Adds a new ModInstallFolder to the instance
        /// </summary>
        /// <param name="path"></param>
        public static void AddModInstallFolder(string path)
        {
            var tempModpackHeader = Automaton.ModpackHeader;
            tempModpackHeader.ModInstallFolders.Add(path);

            Automaton.ModpackHeader = tempModpackHeader;
        }

        /// <summary>
        /// Removes a ModInstallFolder from the instance
        /// </summary>
        /// <param name="path"></param>
        public static void RemoveModInstallFolder(string path)
        {
            var tempModpackHeader = Automaton.ModpackHeader;
            tempModpackHeader.ModInstallFolders.RemoveAll(x => x == path);

            Automaton.ModpackHeader = tempModpackHeader;
        }

        #endregion Modification Methods
    }
}