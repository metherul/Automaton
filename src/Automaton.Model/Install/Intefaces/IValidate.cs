using System.Collections.Generic;
using Automaton.Model.Interfaces;
using Automaton.Model.Modpack.Base;

namespace Automaton.Model.Install.Intefaces
{
    public interface IValidate : IModel
    {
        bool FindMissingMod(Mod mod, List<string> possibleFileMatches);
        List<Mod> GetMissingMods(params string[] directoriesToScan);
    }
}