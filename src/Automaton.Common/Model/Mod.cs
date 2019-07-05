using Automaton.Model.Modpack;
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
    }
}
