using Newtonsoft.Json;
using System.Collections.Generic;
using Automaton.Model.Instance.Interfaces;
using Automaton.Model.ModpackBase.Interfaces;

namespace Automaton.Model.ModpackBase
{
    public class Header : IHeader
    {
        private readonly IAutomatonInstance _automatonInstance;

        public Header(IAutomatonInstance automatonInstance)
        {
            _automatonInstance = automatonInstance;
        }

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

        [JsonProperty("target_game")]
        public string TargetGame { get; set; }

        [JsonProperty("install_mod_organizer")]
        public bool InstallModOrganizer { get; set; }

        [JsonProperty("mod_organizer_version")]
        public int ModOrganizerVersion { get; set; }

        [JsonIgnore]
        public string ModOrganizerArchivePath;

        private string _headerImage;
        [JsonProperty("header_image")]
        public string HeaderImage
        {
            get => System.IO.Path.Combine(_automatonInstance.ModpackExtractionLocation, Extensions.PathExtensions.StandardizePathSeparators(_headerImage));
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
        public string ControlColor { get; set; }

        #endregion Meta Modpack information

        // Contain relative sources to mod install folders. Note that lower indexes are installed before higher ones.

        [JsonProperty("mod_install_folders")]
        public List<string> ModInstallFolders { get; set; }

        // Value must be set to true for the OptionalGUI to be processed and used.
        [JsonProperty("contains_setup_assistant")]
        public bool ContainsSetupAssistant { get; set; } = false;

        [JsonProperty("setup_assistant")]
        public ISetupAssistant SetupAssistant { get; set; }
    }
}