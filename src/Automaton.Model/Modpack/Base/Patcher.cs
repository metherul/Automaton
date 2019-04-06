using Newtonsoft.Json;
using System.Collections.Generic;

namespace Automaton.Model.Modpack.Base
{
    public class Patcher
    {
        [JsonProperty("patch_targets")]
        public List<string> PatchTargets { get; set; }
    }
}
