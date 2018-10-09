using Newtonsoft.Json;
using System.Collections.Generic;
using Automaton.Model.Instance.Interfaces;
using Automaton.Model.ModpackBase.Interfaces;

namespace Automaton.Model.ModpackBase
{
    public class GroupControl : IGroupControl
    {
        private readonly IAutomatonInstance _automatonInstance;

        public GroupControl(IAutomatonInstance automatonInstance)
        {
            _automatonInstance = automatonInstance;
        }

        [JsonProperty("control_type")]
        public Types.ControlType ControlType { get; set; }

        [JsonProperty("control_text")]
        public string ControlText { get; set; }

        [JsonProperty("is_control_checked")]
        public bool? IsControlChecked { get; set; }

        private string _ControlHoverImage;

        [JsonProperty("control_hover_image")]
        public string ControlHoverImage
        {
            get
            {
                if (!string.IsNullOrEmpty(_ControlHoverImage))
                {
                    return System.IO.Path.Combine(_automatonInstance.ModpackExtractionLocation, Extensions.PathExtensions.StandardizePathSeparators(_ControlHoverImage));
                }

                return _ControlHoverImage;
            }
            set => _ControlHoverImage = value;
        }

        [JsonProperty("control_hover_description")]
        public string ControlHoverDescription { get; set; }

        [JsonProperty("control_actions")]
        public List<IFlag> ControlActions { get; set; }
    }
}