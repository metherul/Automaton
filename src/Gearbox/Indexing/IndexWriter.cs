using Gearbox.IO;
using IniParser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.XPath;
using Gearbox.Indexing.Factories;
using Gearbox.Indexing.Interfaces;

namespace Gearbox.Indexing
{
    public partial class IndexWriter
    {
        private readonly Index _indexBase;
        private readonly IndexReader _indexReader;

        public IndexWriter(Index indexBase)
        {
            _indexBase = indexBase;
            
            _indexReader = new IndexReader(_indexBase);
        }

        /// <summary>
        /// Indexes target mod directories.
        /// </summary>
        /// <param name="indexOption">The target behavior of the indexer./param>
        /// <param name="indexModsWithoutMetaInis">Specifies whether mod dirs without valid meta.ini files will be indexed.</param>
        /// <returns></returns>
        public async Task IndexMods(IndexOption indexOption = IndexOption.AllMods, bool indexModsWithoutMetaInis = true)
        {
            var modDirs = (await AsyncFs.GetDirectories(_indexBase.ModsDir));

            foreach (var modDir in modDirs)
            {
                Debug.WriteLine(modDir);
                
                var modIndexer = IndexerFactory.Create(_indexBase, modDir);
                var mod = await modIndexer.Index();

                await JsonUtils.WriteJson(mod, Path.Combine(_indexBase.ModsIndexDir, mod.Name + ".json"));
            }
        }

        /// <summary>
        /// Indexes archives specified by meta.ini Mod Organizer files, as well as all files in param specified directories.
        /// </summary>
        /// <param name="additonalArchiveDirs"></param>
        /// <returns></returns>
        public async Task IndexArchives(params string[] additonalArchiveDirs)
        {
            // Grab archive files from each ini file
            var metaInis = Directory.EnumerateFiles(_indexBase.ModsDir, "meta.ini", SearchOption.AllDirectories);
            var archivePaths = new List<string>();

            foreach (var metaIni in metaInis)
            {
                var parser = new FileIniDataParser();
                var iniData = await Task.Run(() => parser.ReadFile(metaIni));

                var installFile = iniData["General"]["installationFile"];

                if (installFile != null && File.Exists(installFile))
                {
                    archivePaths.Add(PathExtensions.NormalizeFilePath(installFile));
                }
            }

            // archivePaths = archivePaths.OrderByDescending(x => new FileInfo(x).Length).ToList();

            // For each archive, extract, and then index\
            
            foreach (var archivePath in archivePaths)
            {
                var jsonPath = Path.Combine(_indexBase.ArchiveIndexDir, Path.GetFileNameWithoutExtension(archivePath) + ".json");
                var archiveIndexer = IndexerFactory.Create(_indexBase, archivePath);
                var archive = await archiveIndexer.Index();

                await JsonUtils.WriteJson(archive, jsonPath);
            }
        }

        /// <summary>
        /// Indexes files from the game directory itself.
        /// </summary>
        /// <returns></returns>
        public async Task IndexGameDir()  
        {
            // Start indexing the game directory
            var iniParser = new FileIniDataParser();
            var modOrganizerIni = Path.Combine(_indexBase.ModOrganizerDir, "ModOrganizer.ini");
            var iniData = await Task.Run(() => iniParser.ReadFile(modOrganizerIni));
            var gameDir = iniData["General"]["gamePath"];

            var gameDirHeader = HeaderFactory.Create(gameDir, true);
            await gameDirHeader.Build(gameDir);

            await JsonUtils.WriteJson(gameDirHeader, Path.Combine(_indexBase.GameDirIndexDir, "GameDir.json"));
        }

        /// <summary>
        /// Indexes utility files from a specified directory within Mod Organizer (default 
        /// </summary>
        /// <param name="utilitiesDir">The alternative directory to index./param>
        /// <returns></returns>
        public async Task IndexUtilitiesDir(string utilitiesDir = "")
        {
            if (string.IsNullOrEmpty(utilitiesDir))
            {
                utilitiesDir = Path.Combine(_indexBase.ModOrganizerDir, "utilities");
            }

            var utilitiesDirHeader = HeaderFactory.Create(path: utilitiesDir, isGameDir: false, isUtilitiesDir: true);
            await utilitiesDirHeader.Build(utilitiesDir);
        }

        /// <summary>
        /// Updates the mod index with any modifications made to the Mod Organizer mod dir.
        /// </summary>
        /// <param name="indexOption">The target behavior of the updater.</param>
        /// <param name="updateModsWithoutMetaInis">Specifies whether mod dirs without valid meta.ini files will be updated.</param>
        /// <returns></returns>
        public async Task UpdateIndexedMods(IndexOption indexOption, bool updateModsWithoutMetaInis = false)
        {
            var indexMods = await _indexReader.GetModsIndex();
            var modDirs = await AsyncFs.GetDirectories(_indexBase.ModsDir, "*", SearchOption.AllDirectories);

            var indexModPaths = indexMods.Select(x => x.RawPath);
            
            // First we need to check for any mods that have been either removed, or added
            var indexModsToRemove = indexMods.Where(x => !modDirs.Contains(x.RawPath));
            var modDirsToIndex = modDirs.Where(x => !indexModPaths.Contains(x));

            foreach (var mod in indexModsToRemove)
            {
                indexMods.Remove(mod);
            }

            foreach (var modDir in modDirsToIndex) 
            {
                var newHeader = HeaderFactory.Create(modDir);
                await newHeader.Build(modDir);
                
                indexMods.Add(newHeader);
            }
            
            foreach (var indexMod in indexMods)
            {
                var indexEntries = indexMod.IndexEntries.ToList();
                var modDirFiles = await AsyncFs.GetDirectoryFiles(indexMod.RawPath, "*", SearchOption.AllDirectories);

                // Grab any index entries that do not have an existing indexed file or files that have differing last written times.
                var indexEntriesToRemove = indexEntries.Where(x => 
                    (!File.Exists(Path.Combine(indexMod.RawPath, x.RelativeFilePath))) || 
                    (new FileInfo(Path.Combine(indexMod.RawPath, x.RelativeFilePath)).LastWriteTime != x.LastModified));
                
                // Grab any mod files that do not have an index counterpart
                var modFilesToAdd = modDirFiles.Where(x =>
                    (!indexEntries.Select(x => Path.Combine(indexMod.RawPath, x.RelativeFilePath)).Contains(x)));

                foreach (var entry in indexEntriesToRemove)
                {
                    indexEntries.Remove(entry);
                }

                foreach (var modFile in modFilesToAdd)
                {
                    var relativePath = PathExtensions.GetRelativePath(modFile, indexMod.RawPath);
                    var entry = EntryFactory.Create(relativePath, modFile);
                    await entry.Build();
                    
                    indexEntries.Add(entry);
                }

                indexMod.IndexEntries = indexEntries.ToArray();
                await JsonUtils.WriteJson(indexMod, Path.Combine(_indexBase.ModsIndexDir, indexMod.Name + ".json"));
            }
        }

        /// <summary>
        /// Updates the archive index with new archives.
        /// </summary>
        /// <param name="additionalArchiveDirs"></param>
        /// <returns></returns>
        public async Task UpdateIndexedArchives(params string[] additionalArchiveDirs)
        {
            var archiveIndex = await _indexReader.GetArchiveIndex();
            var indexRawPaths = archiveIndex.Select(x => x.RawPath);
            var metaInis = await AsyncFs.GetDirectoryFiles(_indexBase.ModsDir, "meta.ini", SearchOption.AllDirectories);
            var modOrganizerArchives = new List<string>();
            
            foreach (var metaIni in metaInis)
            {
                var iniReader = new FileIniDataParser();
                var parser = iniReader.ReadFile(metaIni);
                var value = parser["General"]["installationFile"];

                if (value != null && File.Exists(value))
                {
                    modOrganizerArchives.Add(parser["General"]["installedFile"]);
                }
            }
            
            var archivesToIndex = modOrganizerArchives.Where(x => indexRawPaths.Contains(x));

            foreach (var archive in archivesToIndex)
            {
                var indexer = IndexerFactory.Create(_indexBase, archive);
                var header = await indexer.Index();

                var jsonPath = Path.Combine(_indexBase.ArchiveIndexDir,
                    Path.GetFileNameWithoutExtension(archive) + ".json");

                await JsonUtils.WriteJson(header, jsonPath);
            }
        }
    }
}
