using Newtonsoft.Json;
using System.Collections.Generic;

namespace Automaton.Model.Modpack
{
    public class ModPackMasterDefinition
    {
        [JsonProperty("standard_target")]
        public string StandardTarget { get; set; }

        [JsonProperty("pack_name")]
        public string PackName { get; set;}

        [JsonProperty("pack_version")]
        public string PackVersion { get; set; }

        [JsonProperty("author")]
        public string AuthorName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("source_url")]
        public string SourceUrl { get; set; }

        [JsonProperty("install_profile_registry")]
        public List<string> InstallProfileRegistry { get; set; }

        [JsonProperty("mo2_path")]
        public string MO2Directory { get; set; }
        [JsonProperty("mo2_profile")]
        public string MO2Profile { get; set; }

        [JsonProperty("alternate_archive_locations")]
        public List<string> AlternateArchiveLocations { get; set; }

        [JsonProperty("scan_game_directory")]
        public bool ScanGameDirectory { get; set; }
        
        public ModPackMasterDefinition Clone()
        {
            return (ModPackMasterDefinition)MemberwiseClone();
        }
    }
}
