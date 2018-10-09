using Automaton.Model.Interfaces;

namespace Automaton.Model.ModpackBase.Interfaces
{
    public interface IFlag : IModel
    {
        Types.FlagActionType FlagAction { get; set; }
        Types.FlagEventType FlagEvent { get; set; }
        string FlagName { get; set; }
        string FlagValue { get; set; }
    }
}