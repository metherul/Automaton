using System;
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

        /// <summary>
        /// Extracts all the files in an archive given a selector
        /// </summary>
        /// <param name="archivePath">Path to an archive</param>
        /// <param name="selector">A function that takes an entry filepath, and returns the outptu path, or null
        /// to skip the file</param>
        void ExtractAll(string archivePath, Func<string, string> selector);
    }
}