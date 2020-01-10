using Gearbox.Indexing.Factories;
using Gearbox.Indexing.Interfaces;
using Gearbox.IO;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Gearbox.Indexing.Indexers
{
    public class ArchiveIndexer : IIndexer
    {
        private readonly IndexBase _indexBase;
        private readonly string _path;

        public ArchiveIndexer(IndexBase indexBase, string path)
        {
            _indexBase = indexBase;
            _path = path;
        }

        public async Task<IIndexHeader> Index()
        {
            var archiveName = Path.GetFileName(_path);
            var extractDir = Path.Combine(_indexBase.ExtractDir, Path.GetFileNameWithoutExtension(archiveName));
            var archive = new ArchiveHandle(_path);

            await archive.Extract(extractDir); 

            var archiveHeader = HeaderFactory.Create(_path);
            await archiveHeader.Build(extractDir);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            await AsyncFs.DeleteDirectory(extractDir, true);

            return archiveHeader;
        }
    }
}
