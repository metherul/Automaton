using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var assembly = Assembly.GetEntryAssembly();
            var resourceName = "Automaton.View.Resources.DLL.7z-x86.dll";

            if (Environment.Is64BitProcess)
            {
                resourceName = "Automaton.View.Resources.DLL.7z-x64.dll";
            }

            var stream = assembly.GetManifestResourceStream(resourceName);
            var dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z.dll");

            if (File.Exists(dllPath))
            {
                File.Delete(dllPath);
            }

            var fileStream = File.Open(dllPath, System.IO.FileMode.CreateNew);

            stream.Seek(0, System.IO.SeekOrigin.Begin);
            stream.CopyTo(fileStream);
            stream.Dispose();

            fileStream.Close();
            fileStream.Dispose();

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
