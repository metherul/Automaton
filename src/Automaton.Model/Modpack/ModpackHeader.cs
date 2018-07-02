using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Automaton.Model.Extensions;
using Automaton.Model.Instances;

namespace Automaton.Model.Modpack
{
    public class ModpackHeader
    {
        #region Meta Modpack information

        [JsonProperty("automaton_version")]
        public string AutomatonVersion { get; set; }

        [JsonProperty("name")]
        public string ModpackName { get; set; }

        [JsonProperty("author")]
        public string ModpackAuthor { get; set; }

        [JsonProperty("version")]
        public string ModpackVersion { get; set; }

        [JsonProperty("source_url")]
        public string ModpackSourceUrl { get; set; }

        [JsonProperty("install_mod_organizer")]
        public bool InstallModOrganizer { get; set; } = false;

        private string _headerImage;

        [JsonProperty("header_image")]
        public string HeaderImage
        {
            get => Path.Combine(ModpackInstance.ModpackExtractionLocation, _headerImage.StandardizePathSeparators());
            set => _headerImage = value;
        }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("background_color_hex")]
        public string BackgroundColor { get; set; }

        [JsonProperty("font_color_hex")]
        public string FontColor { get; set; }

        [JsonProperty("button_color_hex")]
        public string ButtonColor { get; set; }

        [JsonProperty("assistant_control_color_hex")]
        public string AssistantControlColor { get; set; }

        #endregion Meta Modpack information

        // Contain relative sources to mod install folders. Note that lower indexes are installed before higher ones.

        [JsonProperty("mod_install_folders")]
        public List<string> ModInstallFolders { get; set; }

        // Value must be set to true for the OptionalGUI to be processed and used.
        [JsonProperty("contains_setup_assistant")]
        public bool ContainsSetupAssistant { get; set; } = false;

        [JsonProperty("setup_assistant")]
        public SetupAssistant SetupAssistant { get; set; }
    }

    #region Setup Assistant Objects

    public class SetupAssistant
    {
        private string _defaultImage;

        [JsonProperty("default_image")]
        public string DefaultImage
        {
            get => Path.Combine(ModpackInstance.ModpackExtractionLocation, _defaultImage.StandardizePathSeparators());
            set => _defaultImage = value;
        }

        [JsonProperty("default_description")]
        public string DefaultDescription { get; set; }

        [JsonProperty("control_groups")]
        public List<Group> ControlGroups { get; set; }
    }

    public class Group
    {
        [JsonProperty("group_header_text")]
        public string GroupHeaderText { get; set; }

        [JsonProperty("group_controls")]
        public List<GroupControl> GroupControls { get; set; }
    }

    public class GroupControl
    {
        [JsonProperty("control_type")]
        public ControlType ControlType { get; set; }

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
                    return Path.Combine(ModpackInstance.ModpackExtractionLocation, _ControlHoverImage.StandardizePathSeparators());
                }

                return _ControlHoverImage;
            }
            set => _ControlHoverImage = value;
        }

        [JsonProperty("control_hover_description")]
        public string ControlHoverDescription { get; set; }

        [JsonProperty("control_actions")]
        public List<Flag> ControlActions { get; set; }
    }

    public class Flag
    {
        [JsonProperty("flag_name")]
        public string FlagName { get; set; }

        [JsonProperty("flag_value")]
        public string FlagValue { get; set; }

        [JsonProperty("flag_event_type")]
        public FlagEventType FlagEvent { get; set; }

        [JsonProperty("flag_action_type")]
        public FlagActionType FlagAction { get; set; }
    }

    public enum ControlType
    {
        CheckBox,
        RadioButton
    }

    public enum FlagEventType
    {
        Checked,
        UnChecked
    }

    public enum FlagActionType
    {
        Add,
        Remove,
        Subtract,
        Set
    }

    #endregion Optional Installation Objects
}