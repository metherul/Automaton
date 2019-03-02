using System.Collections.Generic;
using Automaton.Model.Install.Intefaces;
using Automaton.Model.Modpack.Base;

namespace Automaton.Model.Install
{
    public class InstallBase : IInstallBase
    {
        public string InstallDirectory { get; set; }
        public List<string> SourceDirectories { get; set; } = new List<string>();

        public Header ModpackHeader { get; set; }
        public List<ExtendedMod> ModpackMods { get; set; } = new List<ExtendedMod>();
    }
}
