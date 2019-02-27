using System.Collections.Generic;
using Automaton.Model.Modpack.Base;
using Newtonsoft.Json;

namespace Automaton.Model.Modpack.Base
{
    public class Mod
    {
        [JsonProperty("mod_name")]
        public string ModName { get; set; }

        [JsonProperty("file_name")]
        public string FileName { get; set; }

        [JsonProperty("file_size")]
        public string FileSize { get; set; }

        [JsonProperty("md5")]
        public string Md5 { get; set; }

        [JsonProperty("nexus_mod_id")]
        public string ModId { get; set; }

        [JsonProperty("nexus_file_id")]
        public string FileId { get; set; }

        [JsonProperty("installation_parameters")]
        public List<InstallParameter> InstallParameters { get; set; }
    }
}
