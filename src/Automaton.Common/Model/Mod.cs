using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Automaton.Common.Model
{
    public class Mod
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("archives")]
        public List<InstallPlan> InstallPlans { get; set; } = new List<InstallPlan>();

        [JsonProperty("mod_type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ModType ModType { get; set; }

        [JsonIgnore]
        public string ModIni { get; set; }
    }
    
    public enum ModType
    {
        InstalledArchive,
        Separator,
        GameDirectoryMod
    }

    public class InstallPlan
    {
        [JsonProperty("source_archive")]
        public SourceArchive SourceArchive { get; set; }

        [JsonProperty("pairings")]
        public List<FilePairing> FilePairings { get; set; }

    }

    public class FilePairing
    {
        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        // Used internally to track patch generation, doesn't exist after modpack creation
        [JsonIgnore]
        public bool is_patched { get; set; }
        [JsonProperty("patch_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string patch_id { get; set; }
    }
}
