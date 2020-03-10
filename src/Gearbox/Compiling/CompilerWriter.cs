using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Gearbox.Indexing;
using Gearbox.Indexing.Interfaces;
using Gearbox.IO;
using Gearbox.Modpacks.Base;
using Gearbox.Modpacks.OMS.Base;
using IniParser;
using Set = Gearbox.Modpacks.Base.Set;

namespace Gearbox.Compiling
{
    public class CompilerWriter
    {
        private List<IIndexHeader> _requiredArchives = new List<IIndexHeader>();
        
        private Index _indexBase;
        private IndexReader _indexReader;
        private ModLinker _modLinker;
        
        private string _modpackName;
        private string _outModpackDir;
        private string _outModsDir;
        private string _outArchivesDir;
        private string _outGameDir;
        private string _outUtilitiesDir;
        
        internal async Task<CompilerWriter> LoadSources(Index indexBase)
        {
            _indexBase = indexBase;
            _indexReader = new IndexReader(indexBase);
            _modLinker = new ModLinker(indexBase);
            var test = await _indexReader.GetArchiveIndex();
            await _modLinker.LoadSources(await _indexReader.GetArchiveIndex());

            return this;
        }
        
        public async Task CompileModpack(CompilerOptions compilerOptions)
        {
            _modpackName = compilerOptions.ModpackName;
            _outModpackDir = Path.Combine(_indexBase.AutomatonDir, "build", _modpackName);
            _outModsDir = Path.Combine(_outModpackDir, "mods");
            _outArchivesDir = Path.Combine(_outModpackDir, "archives");
            _outGameDir = Path.Combine(_outModpackDir, "gamedir");
            _outUtilitiesDir = Path.Combine(_outModpackDir, "utilities");
            
            await CompileMods();
            await CompileGameDir();
            await CompileUtilities();
            await CompileArchives();

            var header = new HeaderBase()
            {
                Name = compilerOptions.ModpackName,
                Author = compilerOptions.Author,
                Version = compilerOptions.Version
            };

            await JsonUtils.WriteJson(header, Path.Combine(_outModpackDir, "header.json"));

            // Now we write the modpack to file
            ZipFile.CreateFromDirectory(_outModpackDir, Path.Combine(_indexBase.AutomatonDir, "build", _modpackName + ".zip"));

            Directory.Delete(_outModpackDir, true);
        }
        
        private async Task CompileMods()
        {
            var modIndex = await _indexReader.GetModsIndex();
            var metaReader = new FileIniDataParser();

            foreach (var indexedMod in modIndex)
            {
                Debug.WriteLine(indexedMod.Name);

                var localRequiredMods = new List<IIndexHeader>();
                var iniFilePath = Path.Combine(_indexBase.ModsDir, indexedMod.Name, "meta.ini");
                IIndexHeader idealSource = null;
                
                var installSets = new List<Set>();
                
                if (File.Exists(iniFilePath))
                {
                    var metaParser = metaReader.ReadFile(iniFilePath);
                    var sourceFilePath = metaParser["General"]["installationFile"] ?? string.Empty;

                    idealSource = _modLinker.GetIdealSource(sourceFilePath);
                }

                foreach (var entry in indexedMod.IndexEntries)
                {
                    var matchResult = await _modLinker.FindBestMatches(entry, idealSource);

                    if (!matchResult.HasValue)
                    {
                        continue;
                    }

                    var temp = matchResult.GetValueOrDefault();

                    if (!_requiredArchives.Contains(temp.SourceHeader))
                    {
                        _requiredArchives.Add(temp.SourceHeader);
                    }

                    if (!localRequiredMods.Contains(temp.SourceHeader))
                    {
                        localRequiredMods.Add(temp.SourceHeader);
                    }
                    
                    installSets.Add(new Set()
                    {
                        Source = $"[{localRequiredMods.IndexOf(temp.SourceHeader)}]{temp.SourceEntry.RelativeFilePath}",
                        Target = entry.RelativeFilePath
                    });
                }

                if (installSets.Count() == 0)
                {
                    continue;
                }
                
                var mod = new Mod()
                {
                    Name = indexedMod.Name,
                    RequiredArchives = localRequiredMods.Select(x => x.Name).ToArray(),
                    Install = installSets.ToArray()
                };

                var jsonPath = Path.Combine(_outModsDir, indexedMod.Name + ".json");
                await JsonUtils.WriteJson(mod, jsonPath);
            }
        }
        
        private async Task CompileArchives()
        {
            foreach (var requiredArchive in _requiredArchives)
            {
                var archive = new Archive()
                {
                    Name = requiredArchive.Name,
                    Hash = await FileHash.GetMd5Async(File.OpenRead(requiredArchive.RawPath)),
                    Length = new FileInfo(requiredArchive.RawPath).Length
                };

                var archiveJsonPath = Path.Combine(_outArchivesDir, Path.GetFileNameWithoutExtension(requiredArchive.Name) + ".json");

                await JsonUtils.WriteJson(archive, archiveJsonPath);
            }
        }
        
        private async Task CompileGameDir()
        {
            var gameDirIndex = await _indexReader.GetGameDirIndex();
            var localRequiredArchives = new List<IIndexHeader>();
            var sets = new List<Set>();

            if (gameDirIndex == null)
            {
                return;
            }

            foreach (var entry in gameDirIndex.IndexEntries)
            {
                var bestMatch = await _modLinker.FindBestMatches(entry);

                if (!bestMatch.HasValue)
                {
                    continue;
                }

                var value = bestMatch.Value;

                if (!localRequiredArchives.Contains(value.SourceHeader))
                {
                    localRequiredArchives.Add(value.SourceHeader);
                }

                if (!_requiredArchives.Contains(value.SourceHeader))
                {
                    _requiredArchives.Add(value.SourceHeader);
                }

                sets.Add(new Set()
                {
                    Source = $"[{localRequiredArchives.IndexOf(value.SourceHeader)}]{value.SourceEntry.RelativeFilePath}",
                    Target = entry.RelativeFilePath
                });
            }

            var mod = new Mod()
            {
                Name = "gameDir",
                RequiredArchives = localRequiredArchives.Select(x => x.Name).ToArray(),
                Install = sets.ToArray()
            };
            
            var outPath = Path.Combine(_outGameDir, "gamedir.json");
            await JsonUtils.WriteJson(mod, outPath);
        }

        private async Task CompileUtilities()
        {
            var utilitiesDirIndex = await _indexReader.GetUtilitiesIndex();
            var localRequiredArchives = new List<IIndexHeader>();
            var sets = new List<Set>();

            if (utilitiesDirIndex == null)
            {
                return;
            }

            foreach (var entry in utilitiesDirIndex.IndexEntries)
            {
                var bestMatch = await _modLinker.FindBestMatches(entry);

                if (!bestMatch.HasValue)
                {
                    continue;
                }

                var value = bestMatch.Value;

                if (!localRequiredArchives.Contains(value.SourceHeader))
                {
                    localRequiredArchives.Add(value.SourceHeader);
                }

                if (!_requiredArchives.Contains(value.SourceHeader))
                {
                    _requiredArchives.Add(value.SourceHeader);
                }

                sets.Add(new Set()
                {
                    Source = $"[{localRequiredArchives.IndexOf(value.SourceHeader)}]{value.SourceEntry.RelativeFilePath}",
                    Target = entry.RelativeFilePath
                });
            }

            var mod = new Mod()
            {
                Name = "utilities",
                RequiredArchives = localRequiredArchives.Select(x => x.Name).ToArray(),
                Install = sets.ToArray()
            };

            var outPath = Path.Combine(_outUtilitiesDir, "utilities.json");
            await JsonUtils.WriteJson(mod, outPath);
        }
    }
}