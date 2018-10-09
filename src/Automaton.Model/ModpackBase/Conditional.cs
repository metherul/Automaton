using Newtonsoft.Json;
using System.ComponentModel;
using Automaton.Model.ModpackBase.Interfaces;

namespace Automaton.Model.ModpackBase
{

    /// <summary>
    /// Flag conditional parameters
    /// </summary>
    public class Conditional : IConditional, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [JsonProperty("conditional_flag_name")]
        public string ConditionalFlagName { get; set; }

        [JsonProperty("conditional_flag_value")]
        public string ConditionalFlagValue { get; set; }
    }
}