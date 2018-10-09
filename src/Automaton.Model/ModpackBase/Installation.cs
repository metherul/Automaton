using Automaton.Model.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using Automaton.Model.ModpackBase.Interfaces;

namespace Automaton.Model.ModpackBase
{

    /// <summary>
    /// Mod installation parameters
    /// </summary>
    public class Installation : INotifyPropertyChanged, IInstallation
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
        public List<IConditional> InstallationConditions { get; set; }
    }
}