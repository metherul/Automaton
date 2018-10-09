using System.Collections.Generic;
using Automaton.Model.Interfaces;

namespace Automaton.Model.ModpackBase.Interfaces
{
    public interface IGroup : IModel
    {
        List<IGroupControl> GroupControls { get; set; }
        string GroupHeaderText { get; set; }
    }
}