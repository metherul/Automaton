using Gearbox.Shared.JsonExt;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Gearbox.SDK.Indexers
{
    public class IndexWriter
    {
        private readonly string _indexFile;
        private readonly string _indexDir;

        private List<ModEntry> _modEntries = new List<ModEntry>();
        private List<ArchiveEntry> _archiveEntries = new List<ArchiveEntry>();

        public IndexWriter(string indexFile)
        {
            _indexFile = indexFile;
            _indexDir = Path.GetDirectoryName(indexFile);
        }

        public void Push(ModEntry modEntry)
        {
            _modEntries.Add(modEntry);
        }

        public void Push(ArchiveEntry archiveEntry)
        {
            _archiveEntries.Add(archiveEntry);
        }

        public async Task Flush()
        {
            var indexRoot = await JsonExt.ReadJson<IndexRoot>(_indexFile);

            indexRoot.ModEntries.AddRange(_modEntries);
            indexRoot.ArchiveEntries.AddRange(_archiveEntries);

            _modEntries = new List<ModEntry>();
            _archiveEntries = new List<ArchiveEntry>();

            await JsonExt.WriteJson(indexRoot, _indexFile);
        }
    }
}
