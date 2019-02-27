using System.Collections.Generic;
using Automaton.Model.Modpack.Base.Interfaces;

namespace Automaton.Model.Modpack.Base
{
    public class ModpackStructure : IModpackStructure
    {
        public List<string> AllPathOffsets { get; set; } = new List<string>(){"header.json", "theme.json", "configurator.json","mods"};

        public string HeaderOffset { get; } = "modpack.json";
        public string ThemeOffset { get; } = "theme.json";
        public string ConfiguratorOffset { get; } = "configurator.json";

        public string ModsInitialDirectoryOffset { get; } = "mods";
    }
}
