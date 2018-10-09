using System.Collections.Generic;
using Automaton.Model.ModpackBase;

namespace Automaton.Model.Instance.Interfaces
{
    public interface IAutomatonInstance : IInstance
    {
        string ExtractionLocation { get; set; }
        string InstallLocation { get; set; }
        string ModOrganizerInstallLocation { get; set; }
        string ModpackExtractionLocation { get; set; }
        Header ModpackHeader { get; set; }
        List<Mod> ModpackMods { get; set; }
        string NexusHandlerRegistryValue { get; set; }
        string PreviousRegistryValue { get; set; }
        string SourceLocation { get; set; }
        string TempDirectory { get; set; }

        void AddModInstallFolder(string path);
        void RemoveModInstallFolder(string path);
    }
}