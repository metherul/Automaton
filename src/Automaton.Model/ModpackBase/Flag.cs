using Newtonsoft.Json;
using Automaton.Model.ModpackBase.Interfaces;

namespace Automaton.Model.ModpackBase
{
    public class Flag : IFlag
    {
        [JsonProperty("flag_name")]
        public string FlagName { get; set; }

        [JsonProperty("flag_value")]
        public string FlagValue { get; set; }

        [JsonProperty("flag_event_type")]
        public Types.FlagEventType FlagEvent { get; set; }

        [JsonProperty("flag_action_type")]
        public Types.FlagActionType FlagAction { get; set; }
    }
}