using System.Collections.Generic;
using Automaton.Model.Interfaces;

namespace Automaton.Model.ModpackBase.Interfaces
{
    public interface IGroupControl : IModel
    {
        List<IFlag> ControlActions { get; set; }
        string ControlHoverDescription { get; set; }
        string ControlHoverImage { get; set; }
        string ControlText { get; set; }
        Types.ControlType ControlType { get; set; }
        bool? IsControlChecked { get; set; }
    }
}