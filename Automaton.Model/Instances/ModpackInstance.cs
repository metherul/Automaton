using System.Collections.Generic;
using Automaton.Model.Modpack;

namespace Automaton.Model.Instances
{
    public class ModpackInstance
    {
        public delegate void ModpackHeaderChanged();
        public static event ModpackHeaderChanged ModpackHeaderChangedEvent;

        private static ModpackHeader _modpackHeader;
        public static ModpackHeader ModpackHeader
        {
            get => _modpackHeader;

            set
            {
                if (_modpackHeader != value && value != null)
                {
                    _modpackHeader = value;

                    ModpackHeaderChangedEvent();
                }
            }
        }

        private static List<Mod> _modpackMods;
        public static List<Mod> ModpackMods
        {
            get => _modpackMods;
            set
            {
                if (_modpackMods != null && _modpackMods != value)
                {
                    _modpackMods = value;
                }
            }
        }

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