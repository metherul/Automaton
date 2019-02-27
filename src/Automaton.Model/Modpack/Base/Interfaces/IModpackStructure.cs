using System.Collections.Generic;
using Automaton.Model.Interfaces;

namespace Automaton.Model.Modpack.Base.Interfaces
{
    public interface IModpackStructure : IModel
    {
        List<string> AllPathOffsets { get; set; }
        string ConfiguratorOffset { get; }
        string HeaderOffset { get; }
        string ModsInitialDirectoryOffset { get; }
        string ThemeOffset { get; }
    }
}