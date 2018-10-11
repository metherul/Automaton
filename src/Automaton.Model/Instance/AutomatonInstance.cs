using Automaton.Model.ModpackBase;
using System;
using System.Collections.Generic;
using System.IO;
using Automaton.Model.Instance.Interfaces;
using Automaton.Model.ModpackBase.Interfaces;

namespace Automaton.Model.Instance
{
    public class AutomatonInstance : IAutomatonInstance
    {
        public IHeader ModpackHeader { get; set; }
        public List<IMod> ModpackMods { get; set; } 

        public string SourceLocation { get; set; }

        public string InstallLocation { get; set; }
        public string ModOrganizerInstallLocation { get; set; }

        public string TempDirectory { get; set; } = Path.GetTempPath();
        public string ExtractionLocation { get; set; } = Path.Combine(Path.GetTempPath(), "Automaton", "extract");
        public string ModpackExtractionLocation { get; set; } = Path.Combine(Path.GetTempPath(), "Automaton", "modpack");

        public string NexusHandlerRegistryValue { get; set; } = $"\"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Automaton.exe")}\" \"%1\"";
        public string PreviousRegistryValue { get; set; }

        #region Modification Methods

        /// <summary>
        /// Adds a new ModInstallFolder to the instance
        /// </summary>
        /// <param name="path"></param>
        public void AddModInstallFolder(string path)
        {
            var tempModpackHeader = ModpackHeader;
            tempModpackHeader.ModInstallFolders.Add(path);

            ModpackHeader = tempModpackHeader;
        }

        /// <summary>
        /// Removes a ModInstallFolder from the instance
        /// </summary>
        /// <param name="path"></param>
        public void RemoveModInstallFolder(string path)
        {
            var tempModpackHeader = ModpackHeader;
            tempModpackHeader.ModInstallFolders.RemoveAll(x => x == path);

            ModpackHeader = tempModpackHeader;
        }

        #endregion Modification Methods
    }
}