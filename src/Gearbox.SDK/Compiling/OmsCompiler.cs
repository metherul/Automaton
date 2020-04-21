using Gearbox.Formats.OMS;
using Gearbox.SDK.Indexers;
using Gearbox.Shared.JsonExt;
using Gearbox.Shared.ModOrganizer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gearbox.SDK
{
    public class OmsCompiler
    {
        private readonly IndexReader _indexReader;
        private readonly ManagerReader _managerReader;

        public OmsCompiler(IndexReader indexReader, ManagerReader managerReader)
        {
            _indexReader = indexReader;
            _managerReader = managerReader;
        }

        public async Task Build(CompilerOptions compilerOptions)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var modEntries = (await compilerOptions.Profile.GetModList())
                .Select(x => _indexReader.GetModByName(x));

            var referencedArchives = new Dictionary<string, ArchiveEntry>();

            foreach (var modEntry in modEntries)
            {
                var entries = modEntry.FileEntries;
                var temp = new List<FileEntry>();
                var sets = new Stack<Set>();
                foreach (var entry in entries)
                {
                    var matches = _indexReader.GetArchiveEntriesByHash(entry.Hash).Reduce(modEntry, entry);

                    if (matches == null)
                    {
                        // At this point we need to generate a patch file.
                        continue;
                    }

                    var set = new Set()
                    {
                        Target = entry.FilePath,
                        Source = matches.FileEntry.FilePath
                    };

                    sets.Push(set);

                    if (!referencedArchives.ContainsKey(matches.SourceArchive.Hash))
                    {
                        referencedArchives.Add(matches.SourceArchive.Hash, matches.SourceArchive);
                    }
                }

                var mod = new Mod()
                {
                    Name = modEntry.Name,
                    ModType = ModType.Manager,
                    Install = sets.ToArray()
                };

                var outPath = Path.Combine(Directory.GetCurrentDirectory(), "test", mod.Name + ".json");
                await JsonExt.WriteJson(mod, outPath);
            }

            stopwatch.Stop();
        }
    }
}