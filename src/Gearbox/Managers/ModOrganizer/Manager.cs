using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Gearbox.IO;
using Gearbox.Modpacks;
using IniParser;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Gearbox.Managers.ModOrganizer
{
    public class Manager : IManager
    {
        private const string Version =
            @"https://github.com/ModOrganizer2/modorganizer/releases/download/v2.2.2.1/Mod.Organizer-2.2.2.1.7z";

        private string _managerDir;
        private string _modDir;
            
        public async Task InstallManager(string installDir)
        {
            var downloadFilePath = Path.GetTempFileName();
            using var webClient = new WebClient();
            await webClient.DownloadFileTaskAsync(new Uri(Version), downloadFilePath);

            var archive = new ArchiveHandle(downloadFilePath);
            await archive.Extract(installDir);

            _managerDir = installDir;
            _modDir = Path.Combine(installDir, "mods");
        }

        public async Task InstallMod(IMod mod) 
        {
            var installEntries = mod.InstallEntries;
            var extractDirs = new List<string>();

            var sourceArchives = installEntries.Select(x => x.Source).Distinct();

            foreach (var sourceArchive in sourceArchives)
            {
                var entries = installEntries.Where(x => x.Source == sourceArchive).Select(x => x.From);
                var archiveHandle = new ArchiveHandle(sourceArchive.FilePath);
                var extractDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "extract", Path.GetFileNameWithoutExtension(sourceArchive.Name));

                await archiveHandle.Extract(entries.ToList(), extractDir);

                extractDirs.Add(extractDir);
            }

            foreach (var entry in installEntries)
            {
                var archivePath = entry.Source.FilePath;
                var extractDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "extract",
                    Path.GetFileNameWithoutExtension(entry.Source.Name));

                var outPath = Path.Combine(_modDir, mod.Name, entry.To);

                if (!Directory.Exists(Path.GetDirectoryName(outPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(outPath));
                }

                if (installEntries.Count(x => x.From == entry.From) > 1)
                {
                    using var fromStream = File.Open(Path.Combine(extractDir, entry.From), FileMode.Open);
                    using var toStream = File.Open(outPath, FileMode.OpenOrCreate);

                    await fromStream.CopyToAsync(toStream);
                }
                else
                {
                    File.Move(Path.Combine(extractDir, entry.From), outPath);
                }

                Debug.WriteLine($"Installed entry: {entry.From} -> {entry.To}");

                if (!extractDirs.Contains(extractDir))
                {
                    extractDirs.Add(extractDir);
                }
            }

            // Write mod metadata.
            var metaDataDir = Path.Combine(_managerDir, "automaton");

            Directory.CreateDirectory(metaDataDir);

            var jsonPath = Path.Combine(metaDataDir, mod.Name + ".json");
            var installMeta = new InstallMetadata()
            {
                TimeOf = DateTime.UtcNow,
                FilesystemHash = (await AsyncFs.GetFilesystemHash(Path.Combine(_modDir, mod.Name)))
            };
            await JsonUtils.WriteJson(installMeta, jsonPath);

            Debug.WriteLine("Cleaning up...");

            foreach (var dir in extractDirs)
            {
                Directory.Delete(dir, true);
            }
        }
    
        public async Task<Option<string>> TryGetGameDir()
        {
            var iniReader = new FileIniDataParser();
            var ini = iniReader.ReadFile(Path.Combine(_managerDir, "ModOrganizer.exe"));

            return None;
        }
    }

    internal class InstallMetadata
    {
        public DateTime TimeOf;
        public string FilesystemHash;
    }
}