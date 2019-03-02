using System.Collections.Generic;
using System.IO;
using System.Linq;
using Automaton.Model.Archive.Interfaces;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;

namespace Automaton.Model.Archive
{
    public class ArchiveContents : IArchiveContents
    {
        public List<ZipArchiveEntry> GetArchiveEntries(string archivePath)
        {
            return ZipArchive.Open(archivePath).Entries.ToList();

            //var archiveFile = new ArchiveFile(archivePath);
            //return archiveFile.Entries.ToList();
        }

        public MemoryStream GetMemoryStreamFromEntry(Entry entry)
        {
            var memoryStream = new MemoryStream();

            return memoryStream;
        }
    }
}
