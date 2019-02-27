using System.Collections.Generic;
using System.IO;
using Automaton.Model.Interfaces;
using SevenZipExtractor;

namespace Automaton.Model.Archive.Interfaces
{
    public interface IArchiveContents : IModel
    {
        List<Entry> GetArchiveEntries(string archivePath);
        MemoryStream GetMemoryStreamFromEntry(Entry entry);
    }
}