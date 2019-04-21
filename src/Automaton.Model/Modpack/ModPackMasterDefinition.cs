using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automaton.Model.Modpack
{
    public class ModPackMasterDefinition
    {
        [JsonProperty("pack_name")]
        public string PackName { get; set;}

        [JsonProperty("author")]
        public string AuthorName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("source_url")]
        public string SourceUrl { get; set; }

        [JsonProperty("mo2_path")]
        public string MO2Directory { get; set; }

        [JsonProperty("mo2_profile")]
        public string MO2Profile { get; set; }

        [JsonProperty("alternate_archive_locations")]
        public List<string> AlternateArchiveLocations { get; set; }


        public ModPackMasterDefinition Clone()
        {
            return (ModPackMasterDefinition)MemberwiseClone();
        }
    }
}
