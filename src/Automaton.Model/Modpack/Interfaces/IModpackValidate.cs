using System.Collections.Generic;
using Automaton.Model.Interfaces;
using SharpCompress.Archives.Zip;

namespace Automaton.Model.Modpack.Interfaces
{
    public interface IModpackValidate : IModel
    {
        bool ValidateCorrectModpackStructure(List<ZipArchiveEntry> modpackEntries);
    }
}