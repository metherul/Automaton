using Newtonsoft.Json;
using System.Collections.Generic;
using Automaton.Model.Instance.Interfaces;
using Automaton.Model.ModpackBase.Interfaces;

namespace Automaton.Model.ModpackBase
{
    public class SetupAssistant : ISetupAssistant
    {
        private readonly IAutomatonInstance _automatonInstance;

        public SetupAssistant(IAutomatonInstance automatonInstance)
        {
            _automatonInstance = automatonInstance;
        }

        private string _defaultImage;

        [JsonProperty("default_image")]
        public string DefaultImage
        {
            get => System.IO.Path.Combine(_automatonInstance.ModpackExtractionLocation, Extensions.PathExtensions.StandardizePathSeparators(_defaultImage));
            set => _defaultImage = value;
        }

        [JsonProperty("default_description")]
        public string DefaultDescription { get; set; }

        [JsonProperty("control_groups")]
        public List<IGroup> ControlGroups { get; set; }
    }
}