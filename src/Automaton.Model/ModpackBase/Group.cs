using Newtonsoft.Json;
using System.Collections.Generic;
using Automaton.Model.Instance.Interfaces;
using Automaton.Model.ModpackBase.Interfaces;

namespace Automaton.Model.ModpackBase
{
    public class Group : IGroup
    {
        [JsonProperty("group_header_text")]
        public string GroupHeaderText { get; set; }

        [JsonProperty("group_controls")]
        public List<IGroupControl> GroupControls { get; set; }
    }
}