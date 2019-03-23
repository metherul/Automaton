using System.Collections.Generic;
using System.IO;
using Automaton.Model.Interfaces;
using SharpCompress.Archives;
using SharpCompress.Common;

namespace Automaton.Model.Archive.Interfaces
{
    public interface IArchiveContents : IModel
    {
        List<IArchiveEntry> GetArchiveEntries(string archivePath);
        MemoryStream GetMemoryStreamFromEntry(Entry entry);
        void ExtractToDirectory(string archivePath, string directoryPath);
    }
}