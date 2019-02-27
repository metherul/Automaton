using System.Collections.Generic;
using Automaton.Model.Interfaces;
using SevenZipExtractor;

namespace Automaton.Model.Modpack.Interfaces
{
    public interface IModpackValidate : IModel
    {
        (bool, string) ValidateCorrectModpackStructure(List<Entry> modpackEntries);
    }
}