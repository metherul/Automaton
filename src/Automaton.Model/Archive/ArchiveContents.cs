using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Automaton.Model.Archive.Interfaces;
using Automaton.Model.Interfaces;
using SharpCompress.Archives;
using SharpCompress.Common;
using Automaton.Model.Install.Intefaces;
using Alphaleonis.Win32.Filesystem;

namespace Automaton.Model.Archive
{
    public class ArchiveContents : IArchiveContents
    {
        private readonly ILogger _logger;
        private readonly ICommonFilesystemUtility _commonFilesystemUtility;
        private readonly IInstallBase _installBase;

        private string _libraryPath;

        public ArchiveContents(IComponentContext components)
        {
            _logger = components.Resolve<ILogger>();
            _commonFilesystemUtility = components.Resolve<ICommonFilesystemUtility>();
            _installBase = components.Resolve<IInstallBase>();

            _libraryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z.dll");
        }

        public IArchive GetArchive(string archivePath)
        {
            return ArchiveFactory.Open(archivePath);
        }

        public List<IArchiveEntry> GetArchiveEntries(IArchive archive)
        {
            return archive.Entries.ToList();
        }

        public System.IO.MemoryStream GetMemoryStreamFromEntry(Entry entry)
        {
            var memoryStream = new System.IO.MemoryStream();

            return memoryStream;
        }


        public void ExtractAll(string archivePath, Func<string, string> selector)
        {
            _logger.WriteLine($"Extracting: {archivePath}");

            using (var archiveFile = new SevenZipExtractor.ArchiveFile(archivePath, _libraryPath))
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

            using (var archiveFile = new SevenZipExtractor.ArchiveFile(archivePath, _libraryPath))
            {
                archiveFile.Extract(delegate (SevenZipExtractor.Entry entry) { return Path.Combine(directoryPath, entry.FileName);});
            }

            return;
        }
    }
}
