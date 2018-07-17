using Newtonsoft.Json;
using System.Collections.Generic;
using Automaton.Model.Extensions;

namespace Automaton.Model.Utility
{
    /// <summary>
    /// The base mod object
    /// </summary>
    public class Mod
    {
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

        [JsonProperty("archive_id")]
        public ArchiveId ArchiveId { get; set; }

        [JsonProperty("installation_parameters")]
        public List<Installation> InstallationParameters { get; set; }
    }

    /// <summary>
    /// Mod installation parameters
    /// </summary>
    public class Installation
    {
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
    public class Conditional
    {
        [JsonProperty("conditional_flag_name")]
        public string ConditionalFlagName { get; set; }

        [JsonProperty("conditional_flag_value")]
        public string ConditionalFlagValue { get; set; }
    }

    /// <summary>
    /// Allows for fast archive identification data to be stored
    /// </summary>
    public class ArchiveId
    {
        [JsonProperty("file_markers")]
        public List<FileMarker> FileMarkers { get; set; }

        [JsonProperty("middle_bytes")]
        public List<string> MiddleBytes { get; set; }

        [JsonProperty("archive_identifier")]
        public string ArchiveIdentifier { get; set; }
    }

    public class FileMarker
    {
        [JsonProperty("byte_index")]
        public string ByteIndex { get; set; }

        [JsonProperty("byte_size")]
        public int ByteSize { get; set; }

        [JsonProperty("byte")]
        public string Byte { get; set; }
    }
}