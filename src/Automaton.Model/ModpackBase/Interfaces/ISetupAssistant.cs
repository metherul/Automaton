using System.Collections.Generic;
using Automaton.Model.Interfaces;

namespace Automaton.Model.ModpackBase.Interfaces
{
    public interface ISetupAssistant : IModel
    {
        List<IGroup> ControlGroups { get; set; }
        string DefaultDescription { get; set; }
        string DefaultImage { get; set; }
    }
}