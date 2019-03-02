using System.Collections.Generic;
using System.IO;
using Automaton.Model.Interfaces;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace Automaton.Model.Archive.Interfaces
{
    public interface IArchiveContents : IModel
    {
        List<ZipArchiveEntry> GetArchiveEntries(string archivePath);
        MemoryStream GetMemoryStreamFromEntry(Entry entry);
    }
}