using Gearbox.Shared.JsonExt;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gearbox.SDK.Indexers
{
    public class IndexWriter
    {
        private readonly string _modIndex;
        private readonly string _archiveIndex;

        private Dictionary<string, ModEntry> _modEntries = new Dictionary<string, ModEntry>(StringComparer.InvariantCultureIgnoreCase);
        private Dictionary<string, ArchiveEntry> _archiveEntries = new Dictionary<string, ArchiveEntry>(StringComparer.InvariantCultureIgnoreCase);

        public IndexWriter(string modIndex, string archiveIndex)
        {
            _modIndex = modIndex;
            _archiveIndex = archiveIndex;
        }

        internal async Task Load()
        {
            var modIndexTask = JsonExt.ReadJson<Dictionary<string, ModEntry>>(_modIndex);
            var archiveIndexTask = JsonExt.ReadJson<Dictionary<string, ArchiveEntry>>(_archiveIndex);

            await Task.WhenAll(modIndexTask, archiveIndexTask);

            _modEntries = modIndexTask.Result;
            _archiveEntries = archiveIndexTask.Result;
        }

        public void Push(ModEntry modEntry)
        {
            if (_modEntries.ContainsKey(modEntry.Name))
            {
                _modEntries[modEntry.Name] = modEntry;

                return;
            }

            _modEntries.Add(modEntry.Name, modEntry);
        }

        public void Push(ArchiveEntry archiveEntry)
        {
            if (_archiveEntries.ContainsKey(archiveEntry.Name))
            {
                _archiveEntries[archiveEntry.Name] = archiveEntry;

                return;
            }

            _archiveEntries.Add(archiveEntry.Name, archiveEntry);
        }

        public async Task Flush()
        {
            var modTask = JsonExt.WriteJson(_modEntries, _modIndex);
            var archiveTask = JsonExt.WriteJson(_archiveEntries, _archiveIndex);

            await Task.WhenAll(modTask, archiveTask);
        }
    }
}
