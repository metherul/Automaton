using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Automaton.Common.Model
{
    public class Manager
    {
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public InstallerType Type { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }
}
