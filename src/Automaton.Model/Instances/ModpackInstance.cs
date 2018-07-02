using System.Collections.Generic;
using Automaton.Model.Modpack;

namespace Automaton.Model.Instances
{
    public class ModpackInstance
    {
        public static ModpackHeader ModpackHeader { get; set; } = new ModpackHeader();

        public static List<Mod> ModpackMods { get; set; } = new List<Mod>();

        private static string _moInstallLocation = "";
        public static string MoInstallLocation
        {
            get => _moInstallLocation;
            set
            {
                if (_moInstallLocation != value)
                {
                    _moInstallLocation = value;

                }
            }
        }

        private static string _sourceLocation = "";
        public static string SourceLocation
        {
            get => _sourceLocation;
            set
            {
                if (_sourceLocation != value)
                {
                    _sourceLocation = value;
                }
            }
        }

        private static string _modpackExtractionLocation = "";
        public static string ModpackExtractionLocation
        {
            get => _modpackExtractionLocation;
            set
            {
                if (_modpackExtractionLocation != value)
                {
                    _modpackExtractionLocation = value;
                }
            }
        }

        #region Modification Methods

        /// <summary>
        /// Adds a new ModInstallFolder to the instance
        /// </summary>
        /// <param name="path"></param>
        public static void AddModInstallFolder(string path)
        {
            var tempModpackHeader = ModpackInstance.ModpackHeader;
            tempModpackHeader.ModInstallFolders.Add(path);

            ModpackInstance.ModpackHeader = tempModpackHeader;
        }

        /// <summary>
        /// Removes a ModInstallFolder from the instance
        /// </summary>
        /// <param name="path"></param>
        public static void RemoveModInstallFolder(string path)
        {
            var tempModpackHeader = ModpackInstance.ModpackHeader;
            tempModpackHeader.ModInstallFolders.RemoveAll(x => x == path);

            ModpackInstance.ModpackHeader = tempModpackHeader;
        }

        #endregion Modification Methods
    }
}