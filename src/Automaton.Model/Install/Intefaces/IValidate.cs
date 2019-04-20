using Automaton.Model.Interfaces;
using System.Collections.Generic;

namespace Automaton.Model.Install.Intefaces
{
    public interface IValidate : IModel
    {
        bool IsArchiveMatch(ExtendedMod mod, string archivePath);
        string BatchFileMatch(ExtendedMod mod, List<string> directoryFiles);
    }
}