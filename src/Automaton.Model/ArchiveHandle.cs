using System;
using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;
using Automaton.Model.Interfaces;
using SevenZipExtractor;

namespace Automaton.Model
{
    public class ArchiveHandle : IArchiveHandle
    {
        private ArchiveFile _archive;

        public IArchiveHandle New(string archivePath)
        {
            var dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z.dll");
            _archive = new ArchiveFile(archivePath, dllPath);

            return this;
        }

        public List<Entry> GetContents()
        {
            return _archive.Entries.ToList();
        }

        public string GetArchiveMd5()
        {
            throw new System.NotImplementedException();
        }

        public void Extract()
        {
            
        }

        public void Extract(List<Entry> entriesToExtract)
        {
            
        }
    }
}
