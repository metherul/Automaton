using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autofac;
using Automaton.Model.Archive.Interfaces;
using Automaton.Model.Interfaces;
using SharpCompress.Archives;
using SharpCompress.Common;
using Alphaleonis.Win32.Filesystem;


namespace Automaton.Model.Archive
{
    public class ArchiveContents : IArchiveContents
    {
        private readonly ILogger _logger;
        private readonly ICommonFilesystemUtility _commonFilesystemUtility;

        public ArchiveContents(IComponentContext components)
        {
            _logger = components.Resolve<ILogger>();
            _commonFilesystemUtility = components.Resolve<ICommonFilesystemUtility>();
        }

        public List<IArchiveEntry> GetArchiveEntries(string archivePath)
        {
            var archive = ArchiveFactory.Open(archivePath);
            var test = archive.Entries.ToList();

            return archive.Entries.ToList();
        }

        public System.IO.MemoryStream GetMemoryStreamFromEntry(Entry entry)
        {
            var memoryStream = new System.IO.MemoryStream();

            return memoryStream;
        }


        public void ExtractAll(string archivePath, Func<String, String> selector)
        {
            _logger.WriteLine($"Extracting: {archivePath}");

            using (var archiveFile = new SevenZipExtractor.ArchiveFile(archivePath))
            {
                archiveFile.Extract(delegate(SevenZipExtractor.Entry entry) { return selector(FixupPath(entry.FileName)); });
            }

            return;
        }

        private string FixupPath(string fileName)
        {
            return "\\" + fileName.Replace("\\\\", "\\");
        }

        public void ExtractToDirectory(string archivePath, string directoryPath)
        {
            _logger.WriteLine($"Extracting: {archivePath} to {directoryPath}");

            using (var archiveFile = new SevenZipExtractor.ArchiveFile(archivePath))
            {
                archiveFile.Extract(directoryPath);
            }

            return;
        }
    }
}
