using Gearbox.Shared.JsonExt;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gearbox.SDK.Indexers
{
    public class IndexReader
    {
        private IndexRoot _index;
        private Dictionary<string, List<MatchResult>> _archiveFileEntries;
        private Dictionary<string, ModEntry> _modEntryDictionary;

        public IReadOnlyList<ModEntry> ModEntries;
        public IReadOnlyList<ArchiveEntry> ArchiveEntries;

        public async Task LoadIndex(string indexDir)
        {
            var archiveIndex = Path.Combine(indexDir, "archives.index");
            var modsIndex = Path.Combine(indexDir, "mods.index");
            _index = await JsonExt.ReadJson<IndexRoot>(indexDir);

            // Construct dictionaries for quick MD5 hash search.
            var archiveFiles = _index.ArchiveEntries.SelectMany(x => x.FileEntries.Select(y => new MatchResult()
            {
                SourceArchive = x,
                FileEntry = y
            }));
            var groupedArchiveFiles = archiveFiles.GroupBy(x => x.FileEntry.Hash);
            _archiveFileEntries = groupedArchiveFiles.ToDictionary(x => x.Key, x => x.ToList());

            _modEntryDictionary = _index.ModEntries.ToDictionary(x => x.Name, x => x);

            ModEntries = _index.ModEntries;
            ArchiveEntries = _index.ArchiveEntries;
        }

        public List<MatchResult> GetArchiveEntriesByHash(string md5)
        {
            return _archiveFileEntries.GetValueOrDefault(md5.ToLower()) ?? new List<MatchResult>();
        }

        public ModEntry GetModByName(string modName)
        {
            return _modEntryDictionary.GetValueOrDefault(modName);
        }
    }
}
