using System.Collections.Generic;
using System.IO;
using System.Linq;
using Automaton.Model.Archive.Interfaces;
using SevenZipExtractor;

namespace Automaton.Model.Archive
{
    public class ArchiveContents : IArchiveContents
    {
        public List<Entry> GetArchiveEntries(string archivePath)
        {
            var archiveFile = new ArchiveFile(archivePath);
            return archiveFile.Entries.ToList();
        }

        public MemoryStream GetMemoryStreamFromEntry(Entry entry)
        {
            var memoryStream = new MemoryStream();
            entry.Extract(memoryStream);

            return memoryStream;
        }
    }
}
