using System.Collections.Generic;
using System.ComponentModel;
using Automaton.Model.Extensions;
using Newtonsoft.Json;

namespace Automaton.Model.ModpackBase
{
    /// <summary>
    /// The base mod object
    /// </summary>
    public class Mod : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [JsonProperty("mod_name")]
        public string ModName { get; set; }

        [JsonProperty("mod_archive_name")]
        public string ModArchiveName { get; set; }

        [JsonProperty("mod_archive_size")]
        public string ModArchiveSize { get; set; }

        [JsonIgnore]
        public string ModArchivePath { get; set; }

        [JsonIgnore]
        public string ModInstallParameterPath { get; set; }

        [JsonProperty("archive_md5sum")]
        public string ArchiveMd5Sum { get; set; }

        [JsonProperty("mod_source_url")]
        public string ModSourceUrl { get; set; }

        [JsonProperty("nexus_mod_id")]
        public string NexusModId { get; set; }
        
        [JsonIgnore]
        public int CurrentDownloadPercentage { get; set; } // Temporary fix, will work out later

        [JsonProperty("installation_parameters")]
        public List<Installation> InstallationParameters { get; set; }
    }

    /// <summary>
    /// Mod installation parameters
    /// </summary>
    public class Installation : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _sourceLocation;
        [JsonProperty("source_location")]
        public string SourceLocation
        {
            get => _sourceLocation.StandardizePathSeparators();
            set => _sourceLocation = value;
        }

        private string _targetLocation;
        [JsonProperty("target_location")]
        public string TargetLocation
        {
            get => _targetLocation.StandardizePathSeparators();
            set => _targetLocation = value;
        }

        [JsonProperty("installation_conditions")]
        public List<Conditional> InstallationConditions { get; set; }
    }

    /// <summary>
    /// Flag conditional parameters
    /// </summary>
    public class Conditional : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [JsonProperty("conditional_flag_name")]
        public string ConditionalFlagName { get; set; }

        [JsonProperty("conditional_flag_value")]
        public string ConditionalFlagValue { get; set; }
    }
}