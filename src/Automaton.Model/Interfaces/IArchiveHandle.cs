using SevenZipExtractor;
using System.Collections.Generic;

namespace Automaton.Model.Interfaces
{
    public interface IArchiveHandle : IModel
    {
        IArchiveHandle New(string archivePath);
        string GetArchiveMd5();
        List<Entry> GetContents();
        void Extract();
        void Extract(List<Entry> entriesToExtract);
    }
}
