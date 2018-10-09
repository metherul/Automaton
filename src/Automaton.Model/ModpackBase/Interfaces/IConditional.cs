using System.ComponentModel;
using Automaton.Model.Interfaces;

namespace Automaton.Model.ModpackBase.Interfaces
{
    public interface IConditional : IModel
    {
        string ConditionalFlagName { get; set; }
        string ConditionalFlagValue { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}