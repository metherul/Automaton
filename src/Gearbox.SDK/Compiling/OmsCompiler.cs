using Gearbox.Formats.OMS;
using Gearbox.SDK.Indexers;
using Gearbox.Shared.JsonExt;
using Gearbox.Shared.ModOrganizer;
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

            var mods = new List<Mod>();
            var archives = new List<Archive>();

            var referencedArchives = new Dictionary<string, ArchiveEntry>();

            foreach (var modEntry in modEntries)
            {
                var preferredArchive = Path.GetFileName(_managerReader.GetModSourceArchive(modEntry.Name));
                var entries = modEntry.FileEntries;
                var temp = new List<FileEntry>();
                var sets = new Stack<Set>();
                foreach (var entry in entries)
                {
                    // Grab our possible matches by hash and reduce to one by comparing to the current modEntry.
                    var matches = _indexReader.GetArchiveEntriesByHash(entry.Hash)
                        .Reduce(modEntry, entry, new ReduceOptions { PreferredArchiveName = preferredArchive });

                    // TODO
                    // No matches, so we need to implement an auxillary matcher / patching method.
                    if (matches == null)
                    {
                        continue;
                    }

                    // Begin mapping the matchResult object to the OMS pack Set object.
                    var set = new Set()
                    {
                        Target = entry.FilePath,
                        Source = matches.FileEntry.FilePath,
                        SourceArchiveHash = matches.SourceArchive.Hash
                    };

                    sets.Push(set);

                    // Begin coelescing archives into a dictionary so there are no duplicates.
                    if (!referencedArchives.ContainsKey(matches.SourceArchive.Hash))
                    {
                        referencedArchives.Add(matches.SourceArchive.Hash, matches.SourceArchive);

                        // Map the archiveEntry to an OMS Archive object.
                        var archive = new Archive()
                        {
                            FileName = matches.SourceArchive.Name,
                            Hash = matches.SourceArchive.Hash,
                            FilesystemHash = matches.SourceArchive.FilesystemHash,
                            Length = matches.SourceArchive.Length
                        };

                        archives.Add(archive);
                    }
                }

                // Map mod compilation results to the OMS Mod object.
                var mod = new Mod()
                {
                    Name = modEntry.Name,
                    ModType = ModType.Manager,
                    Install = sets.ToArray()
                };

                mods.Add(mod);
            }

            var outDir = Path.Combine(Directory.GetCurrentDirectory(), "working");
            var outMods = Path.Combine(outDir, "mods.json");
            var outArchives = Path.Combine(outDir, "archives.json");

            var outModsTask = JsonExt.WriteJson(mods, outMods);
            var outArchivesTask = JsonExt.WriteJson(archives, outArchives);

            await Task.WhenAll(outModsTask, outArchivesTask);

            stopwatch.Stop();
        }
    }
}