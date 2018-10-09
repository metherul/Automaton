using System.Collections.Generic;
using System.ComponentModel;
using Automaton.Model.Interfaces;

namespace Automaton.Model.ModpackBase.Interfaces
{
    public interface IInstallation : IModel
    {
        List<IConditional> InstallationConditions { get; set; }
        string SourceLocation { get; set; }
        string TargetLocation { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}