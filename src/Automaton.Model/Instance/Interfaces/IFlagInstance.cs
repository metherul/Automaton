using System.Collections.Generic;
using Automaton.Model.ModpackBase;

namespace Automaton.Model.Instance.Interfaces
{
    public interface IFlagInstance : IInstance
    {
        List<FlagKeyValue> FlagKeyValueList { get; set; }

        void AddOrModifyFlag(string flagKey, string flagValue, FlagActionType flagActionType);
    }
}