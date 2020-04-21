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

            // Grab the mod names from the current profile and load each's index equivalent.
            var modEntries = (await compilerOptions.Profile.GetModList())
                .Select(x => _indexReader.GetModByName(x));

            var referencedArchives = new Dictionary<string, ArchiveEntry>();

            foreach (var modEntry in modEntries)
            {
                var preferredArchive = Path.GetFileName(_managerReader.GetModSourceArchive(modEntry.Name));
                var entries = modEntry.FileEntries;
                var temp = new List<FileEntry>();
                var sets = new Stack<Set>();
                var archives = new List<ArchiveEntry>();
                foreach (var entry in entries)
                {
                    // Grab our possible matches by hash and reduce to one by comparing to the current modEntry.
                    var matches = _indexReader.GetArchiveEntriesByHash(entry.Hash)
                        .Reduce(modEntry, entry, new ReduceOptions { PreferredArchiveName = preferredArchive });

                    // No matches, so we need to implement an auxillary matcher / patching method.
                    if (matches == null)
                    {
                        // At this point we need to generate a patch file.
                        continue;
                    }

                    // Begin mapping the matchResult object to the OMS pack Set object.
                    var set = new Set()
                    {
                        Target = entry.FilePath,
                        Source = matches.FileEntry.FilePath
                    };

                    sets.Push(set);

                    // Begin coelescing archives into a dictionary so there are no duplicates.
                    if (!referencedArchives.ContainsKey(matches.SourceArchive.Hash))
                    {
                        referencedArchives.Add(matches.SourceArchive.Hash, matches.SourceArchive);
                    }
                }

                // Map mod compilation results to the OMS Mod object.
                var mod = new Mod()
                {
                    Name = modEntry.Name,
                    ModType = ModType.Manager,
                    Install = sets.ToArray()
                };

                // Write the new Mod object into a .json document.
                // This is a temporary solution for performance testing. The format will most likely change.
                var outPath = Path.Combine(Directory.GetCurrentDirectory(), "test", mod.Name + ".json");
                await JsonExt.WriteJson(mod, outPath);
            }

            stopwatch.Stop();
        }
    }
}