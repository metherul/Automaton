using System.Collections.Generic;
using Newtonsoft.Json;

namespace Automaton.Model.Modpack.Base
{
    public class Header 
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("author")]
        public string Author { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("source_url")]
        public string SourceUrl { get; set; }
        [JsonProperty("target_game")]
        public string TargetGame { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("install_mod_organizer")]
        public bool InstallModOrganizer { get; set; }

        [JsonProperty("mod_install_folders")]
        public List<string> ModInstallFolders { get; set; }
    }
}
