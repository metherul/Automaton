using System.Collections.Generic;
using Newtonsoft.Json;

namespace Automaton.Model.Modpack.Base
{
    public class InstallParameter
    {
        [JsonProperty("source_location")]
        public string SourceLocation { get; set; }

        [JsonProperty("target_location")]
        public string TargetLocation { get; set; }
    }
}
