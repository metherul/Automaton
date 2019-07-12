using Automaton.Common;
using Hephaestus.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Hephaestus.Nexus;
using System.IO.Compression;
using Automaton.Common.Model;
using SevenZipExtractor;
using System.Net.Http;

namespace Hephaestus
{
    public class PackBuilder
    {
        public MasterDefinition ModPackMasterDefinition { get; private set; }
        public Preferences Preferences { get; private set; }
        public NexusClient NexusClient { get; private set; }
        public dynamic MO2Ini { get; set; }
        public string DefaultGame { get; set; }
        public List<string> ModListData { get; set; }
        public List<Mod> CompiledMods { get; private set; }



        public void LoadPrefs(string filepath)
        {
            //Preferences = Utils.LoadJson<Preferences>(filepath);
            NexusClient = new NexusClient(Utils.GetNexusAPIKey());
        }

        public IList<InstalledMod> InstalledMods { get; private set; }
        public IList<Model.SourceArchive> SourceArchives { get; private set; }

        public readonly ISet<string> SupportedArchives = new HashSet<string>() { ".7z", ".7zip", ".rar", ".zip" };

        public void CompilePatches()
        {
            // Create a folder for patches
            string patch_folder = "./temp_patches";
            Directory.CreateDirectory(patch_folder);

            var sources = SourceArchives.GroupBy(d => d.SHA256).ToDictionary(d => d.Key);

            var archives_with_patches = (from mod in CompiledMods
                                         from plan in mod.InstallPlans
                                         from pairing in plan.FilePairings
                                         where pairing.is_patched == true
                                         select (sources[plan.SourceArchive.SHA256].First(), pairing, mod)).GroupBy(x => x.Item1);

            Log.Info("Generating patches from {0} archives", archives_with_patches.Count());



            archives_with_patches.AsParallel()
                                 .Select(x => GeneratePatchsForArchive(patch_folder, x.Key, x.Select(p => (p.pairing, p.mod))))
                                 .ToList();
                           

        }

        private Model.SourceArchive GeneratePatchsForArchive(string patch_folder, Model.SourceArchive archive, IEnumerable<(FilePairing pairing, Mod mod)> pairings)
        {
            var need_patches = pairings.Where(p => p.pairing.is_patched).ToList();


            foreach (var pairing in need_patches)
            {
                pairing.pairing.patch_id = Guid.NewGuid().ToString();
            }

            var selected_files = need_patches.Select(n => n.pairing.From).ToHashSet();

            Dictionary<string, MemoryStream> extracted = new Dictionary<string, MemoryStream>();

            using (var file = new ArchiveFile(archive.FullPath))
            {
                file.Extract(e => {
                    if (selected_files.Contains(e.FileName))
                    {
                        var stream = new MemoryStream();
                        extracted.Add(e.FileName, stream);
                        return stream;
                    }
                    return null;

                });
            }

            foreach (var pairing in need_patches)
            {
                Log.Info("Generating Patch for: {0} ", Path.GetFileName(pairing.pairing.To));
                var ss = extracted[pairing.pairing.From];
                var patched_file = Path.Combine(ModsFolder, pairing.mod.Name, pairing.pairing.To);
                using (var origin = new MemoryStream(ss.ToArray())) 
                using (var dest = File.OpenRead(patched_file))
                using (var output = File.OpenWrite(Path.Combine(patch_folder, pairing.pairing.patch_id)))
                {
                    var a = origin.ReadAll();
                    var b = dest.ReadAll();
                    BSDiff.Create(a, b, output);
                }

            }


            return archive;
        }

        public void CleanupPatches()
        {
            Directory.Delete("./temp_patches", true);
        }

        public PackBuilder() { }

        public string ModsFolder
        {
            get
            {
                return Path.Combine(ModPackMasterDefinition.MO2Directory, "mods");
            }
        }

        public string PackFileName
        {
            get
            {
                return ModPackMasterDefinition.MO2Profile + ".auto";
            }
        }

        public string DownloadsFolder
        {
            get
            {
                return Path.Combine(ModPackMasterDefinition.MO2Directory, "downloads");
            }
        }

        public string ProfileFolder
        {
            get
            {
                return Path.Combine(ModPackMasterDefinition.MO2Directory, "profiles", ModPackMasterDefinition.MO2Profile);
            }
        }

        public string ModListFileName
        {
            get {
                return Path.Combine(ProfileFolder, "modlist.txt");
            }
        }

        public IEnumerable<string> ArchiveLocations
        {
            get
            {
                yield return DownloadsFolder;
                if (ModPackMasterDefinition.AlternateArchiveLocations != null) {
                foreach (var folder in ModPackMasterDefinition.AlternateArchiveLocations)
                    {
                        yield return folder;
                    }
                }
            }
        }

        public Dictionary<string, IGrouping<string, (Model.SourceArchive archive, ArchiveEntry file)>> IndexedArchives { get; private set; }

        public void FindArchives()
        {
            Log.Info("Finding archives");
            var from_mods = (from location in ArchiveLocations
                             from file in Directory.EnumerateFiles(location)
                             where SupportedArchives.Contains(Path.GetExtension(file))
                             select file).ToList();

            var archives = from_mods.AsParallel()
                                    .Select(file => Model.SourceArchive.FromFileName(this, file))
                                    .ToList();
            SourceArchives = archives;

            Log.Info("Found {0} archives", SourceArchives.Count);


        }

        public void LoadPackDefinition(string path)
        {
            ModPackMasterDefinition = Utils.LoadJson<MasterDefinition>(path);
        }

        public void LoadInstalledMods()
        {
            InstalledMods = Directory.EnumerateDirectories(ModsFolder)
                            .AsParallel()
                            .Select(mod => InstalledMod.FromFolder(mod))
                            .ToList();
                            
        }

        public void LoadMO2Data()
        {
            MO2Ini = Utils.LoadIni(Path.Combine(ModPackMasterDefinition.MO2Directory, "ModOrganizer.ini"));
            DefaultGame = MO2Ini.General.gameName;
            if (string.IsNullOrEmpty(DefaultGame))
                Log.HardError("No Default game found");

            ModListData = (from line in File.ReadAllLines(ModListFileName)
                           where line.StartsWith("+") || (line.StartsWith("-") && line.EndsWith("_separator"))
                           select line.Substring(1)).ToList();
        }

        public void CompileMods()
        {
            var enabled_mods = ModListData.ToHashSet();

            IndexedArchives = (from archive in SourceArchives
                                from file in archive.ArchiveEntries
                                group (archive: archive, file: file) by file.SHA256 into grouped
                                select grouped).ToDictionary(k => k.Key);

            var compiled_mods = (from mod in InstalledMods.AsParallel()
                                 where enabled_mods.Contains(mod.ModName)
                                 select CompileMod(mod)).ToList();

            CompiledMods = compiled_mods;
            
        }

        private Mod CompileMod(InstalledMod mod)
        {
            var compiled_mod = new Mod();
            compiled_mod.Name = mod.ModName;
            compiled_mod.InstallPlans = new List<InstallPlan>();

            Log.Info("Compiling {0}", mod.ModName);

            var primary_source = (from archive in SourceArchives
                                 where archive.ModId == mod.MetaData.General.modid
                                 where archive.ArchiveName == mod.MetaData.General.installationFile
                                 select archive).FirstOrDefault();

            if (mod.ModName.EndsWith("_separator"))
            {
                compiled_mod.ModType = ModType.Separator;

            }
            else
            {
                compiled_mod.ModType = ModType.InstalledArchive;
                ScanDirectory(mod.FullPath, compiled_mod, primary_source, null, false);
            }
            Log.Info("Done Compiling {0}", mod.ModName);
            return compiled_mod;
        }

        private void ScanDirectory(string full_path, Mod compiled_mod, Model.SourceArchive primary_source, ISet<string> ignore, bool supress_warning)
        {
            foreach (var file in Directory.EnumerateFiles(full_path, "*", SearchOption.AllDirectories))
            {
                if (Path.GetFileName(file) == "meta.ini") continue;
                if (ignore != null && ignore.Contains(Path.GetExtension(file))) continue;

                var sha = Utils.FileSHA256(file);
                var to_path = Utils.StripPrefix(full_path, file);

                // Exact match
                if (IndexedArchives.TryGetValue(sha, out var entries))
                {
                    // Find the archive with the same name as our primary_source
                    var pair = (from entry in entries
                                where entry.archive == primary_source
                                select entry).FirstOrDefault();

                    // Or default to the first archive
                    if (pair.archive == null)
                    {
                        pair = entries.First();
                    }

                    AddPairToMod(compiled_mod, to_path, pair.archive, pair.file);
                    continue;

                }


                // Find a file in the primary source with the same name?
                if (primary_source != null)
                {
                    var primary_source_file = (from archive_file in primary_source.ArchiveEntries
                                               where Path.GetFileName(archive_file.FileName) == Path.GetFileName(archive_file.FileName)
                                               select archive_file).FirstOrDefault();

                    if (primary_source_file != null)
                    {
                        Log.Info("Found name match for {0} in primary mod source, building patch.", file);

                        var pairing = AddPairToMod(compiled_mod, to_path, primary_source, primary_source_file);
                        pairing.is_patched = true;
                        continue;
                    }
                }
                
                if (!supress_warning)
                {
                    Log.Warn("No match: {0}", file);
                }

            }
        }

        private static FilePairing AddPairToMod(Mod compiled_mod, string to_path, Model.SourceArchive archive, ArchiveEntry file)
        {
            var install_plan = compiled_mod.InstallPlans.FirstOrDefault(v => v.SourceArchive.SHA256 == archive.SHA256);
            if (install_plan == null)
            {
                install_plan = new InstallPlan();
                compiled_mod.InstallPlans.Add(install_plan);

                install_plan.SourceArchive = new Automaton.Common.Model.SourceArchive();
                install_plan.FilePairings = new List<FilePairing>();
                Utils.MemberwiseCopy(archive, install_plan.SourceArchive);

            }
            var pairing = new FilePairing()
            {
                From = file.FileName,
                To = to_path
            };
            install_plan.FilePairings.Add(pairing);
            return pairing;
        }

        public void CompileGameDirectory()
        {
            if (!ModPackMasterDefinition.ScanGameDirectory) return;
            var game_dir_mod = new Mod();
            CompiledMods.Add(game_dir_mod);
            game_dir_mod.ModType = ModType.GameDirectoryMod;
            game_dir_mod.Name = "Game Directory Files";
            Log.Info("Scanning Game Folder");
            ScanDirectory(MO2Ini.General.gamePath, game_dir_mod, null, new HashSet<string>() { ".bsa", ".psc", ".bak" }, true);
        }

        public void ExportPack()
        {

            if (File.Exists(PackFileName))
                File.Delete(PackFileName);

            using (var zip = ZipFile.Open(PackFileName, ZipArchiveMode.Create))
            {
                Log.Info("Exporting Header");
                var master = ModPackMasterDefinition.Clone();
                master.MO2Directory = null;
                master.AlternateArchiveLocations = new List<string>();
                master.MO2Archive = GetMO2ARchiveInfo();
                Utils.SpitJsonInto(zip, "pack.auto_definition", master);

                foreach (var mod in CompiledMods)
                {
                    if (mod.ModType == ModType.GameDirectoryMod)
                    {
                        Log.Info("Exporting Game Directory Files");
                        Utils.SpitJsonInto(zip, "game_folder_mod.json", mod);
                    }
                    else
                    {
                        Log.Info("Exporting {0}", mod.Name);
                        Utils.SpitJsonInto(zip, Path.Combine("mods", mod.Name, "install.json"), mod);
                        zip.CreateEntryFromFile(Path.Combine(ModsFolder, mod.Name, "meta.ini"), Path.Combine("mods", mod.Name, "meta.ini"));
                    }
                }

                string full_path;
                foreach (var file in Directory.EnumerateFiles(ProfileFolder).Where(f => File.Exists(f)))
                {
                    if (Path.GetFileName(file) == "modlist.txt") continue;

                    full_path = Path.Combine("profile", Path.GetFileName(file));
                    zip.CreateEntryFromFile(file, full_path);
                    
                }

                // Scrub the mod list of disabled mods
                var mod_list = File.ReadAllLines(Path.Combine(ProfileFolder, "modlist.txt"))
                                   .Where(x => !x.StartsWith("-") || x.EndsWith("_separator"))
                                   .ToArray();
                
                full_path = Path.Combine("profile", "modlist.txt");
                Utils.SpitInto(zip, full_path, String.Join("\n", mod_list));


                // Write Patches
                foreach (var file in Directory.EnumerateFiles("./temp_patches"))
                {
                    zip.CreateEntryFromFile(file, "patches\\" + Path.GetFileName(file));
                }

                // Write Mod metadata
                foreach (var file in Directory.EnumerateFiles(ProfileFolder, "*.meta"))
                {
                    zip.CreateEntryFromFile(file, Path.GetFileName(file));
                }

            }
        }


        class GitHubRelease
        {
            public bool prerelease;
            public List<GitHubAsset> assets { get; set; }
        }

        class GitHubAsset
        {
            public string name;
            public long size;
            public string browser_download_url;
        }

        public Automaton.Common.Model.SourceArchive GetMO2ARchiveInfo()
        {
            Log.Info("Loading latest MO2 Archive Info from GitHub");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "request");
            var result = client.GetStreamAsync("https://api.github.com/repos/ModOrganizer2/modorganizer/releases");
            result.Wait();
            var releases = Utils.LoadJson<List<GitHubRelease>>(result.Result);
            var use_archive = (from release in releases
                               where !release.prerelease
                               from asset in release.assets
                               where !asset.name.EndsWith(".exe")
                               select asset).First();

            var archive = new Automaton.Common.Model.SourceArchive();
            archive.Name = use_archive.name;
            archive.ArchiveName = use_archive.name;
            archive.Repository = "GitHub";
            archive.DirectURL = use_archive.browser_download_url;

            Log.Info("Hashing latest MO2");
            result = client.GetStreamAsync(archive.DirectURL);
            result.Wait();
            archive.SHA256 = Utils.SHA256(result.Result); ;
            archive.Size = use_archive.size;
            return archive;

        }

    }
}
