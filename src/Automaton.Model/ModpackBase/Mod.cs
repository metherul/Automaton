using Automaton.Model.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using Automaton.Model.ModpackBase.Interfaces;

namespace Automaton.Model.ModpackBase
{
    /// <summary>
    /// The base mod object
    /// </summary>
    public class Mod : IMod, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [JsonProperty("mod_name")]
        public string ModName { get; set; }

        [JsonProperty("file_name")]
        public string FileName { get; set; }

        [JsonProperty("file_size")]
        public string FileSize { get; set; }

        [JsonIgnore]
        public string FilePath { get; set; }

        [JsonIgnore]
        public string ModInstallParameterPath { get; set; }

        [JsonProperty("md5")]
        public string Md5 { get; set; }

        [JsonProperty("mod_source_url")]
        public string ModSourceUrl { get; set; }

        [JsonProperty("nexus_mod_id")]
        public string NexusModId { get; set; }

        [JsonIgnore]
        public int CurrentDownloadPercentage { get; set; } // Temporary fix, will work out later

        [JsonIgnore]
        public bool IsIndeterminateProcess { get; set; }

        [JsonProperty("installation_parameters")]
        public List<IInstallation> InstallationParameters { get; set; }
    }
}