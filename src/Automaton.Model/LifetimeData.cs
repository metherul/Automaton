using Automaton.Common.Model;
using Automaton.Model.Interfaces;
using System.Collections.Generic;

namespace Automaton.Model
{
    public class LifetimeData : ILifetimeData
    {
        public MasterDefinition MasterDefinition { get; set; }
        public List<Mod> Mods { get; set; }
        public List<ModpackItem> ModpackContent { get; set; }
        public string RequestHeader { get; set; }
        public string InstallPath { get; set; }
        public string DownloadPath { get; set; }
        public string ApiKey { get; set; }
    }
}
