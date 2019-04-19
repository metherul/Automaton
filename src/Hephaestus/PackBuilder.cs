using Automaton.Model.Modpack;
using Automaton.Utils;
using Hephaestus.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Hephaestus.Nexus;
using System.IO.Compression;

namespace Hephaestus
{
    public class PackBuilder
    {
        public ModPackMasterDefinition ModPackMasterDefinition { get; private set; }
        public Preferences Preferences { get; private set; }
        public NexusClient NexusClient { get; private set; }
        public dynamic MO2Ini { get; set; }
        public string DefaultGame { get; set; }
        public List<string> ModListData { get; set; }
        public List<CompiledMod> CompiledMods { get; private set; }



        public void LoadPrefs(string filepath)
        {
            Preferences = Utils.LoadJson<Preferences>(filepath);
            NexusClient = new NexusClient(Preferences.ApiKey);
        }

        public IList<InstalledMod> InstalledMods { get; private set; }
        public IList<SourceArchive> SourceArchives { get; private set; }

        public readonly ISet<string> SupportedArchives = new HashSet<string>() { ".7z", ".7zip", ".rar", ".zip" };


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

        public void FindArchives()
        {
            var from_mods = (from location in ArchiveLocations
                             from file in Directory.EnumerateFiles(location)
                             where SupportedArchives.Contains(Path.GetExtension(file))
                             select file).ToList();

            var archives = from_mods.AsParallel()
                                    .Select(file => SourceArchive.FromFileName(this, file))
                                    .ToList();
            SourceArchives = archives;

            Log.Info("Found {0} archives", SourceArchives.Count);


        }

        public void LoadPackDefinition(string path)
        {
            ModPackMasterDefinition = Utils.LoadJson<ModPackMasterDefinition>(path);
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

            var indexed_files = (from archive in SourceArchives
                                 from file in archive.ArchiveEntries
                                 group (archive: archive, file: file) by file.SHA256 into grouped
                                 select grouped).ToDictionary(k => k.Key);

            var compiled_mods = (from mod in InstalledMods
                                 where enabled_mods.Contains(mod.ModName)
                                 select CompileMod(mod, indexed_files)).ToList();

            CompiledMods = compiled_mods;
            
        }

        private CompiledMod CompileMod(InstalledMod mod, IDictionary<string, IGrouping<string, (SourceArchive archive, ArchiveEntry file)>> indexed)
        {
            var compiled_mod = new CompiledMod();
            compiled_mod.Name = mod.ModName;
            compiled_mod.InstallPlans = new List<InstallPlan>();
            compiled_mod.RawINI = Utils.Slurp(Path.Combine(mod.FullPath, "meta.ini"));

            Log.Info("Compiling {0}", mod.ModName);

            var primary_source = (from archive in SourceArchives
                                 where archive.ModId == mod.MetaData.General.modid
                                 where archive.ArchiveName == mod.MetaData.General.installationFile
                                 select archive).FirstOrDefault();

            if (mod.ModName.EndsWith("_separator"))
            {
                compiled_mod.IsSeparator = true;

            }
            else
            {

                foreach (var file in Directory.EnumerateFiles(mod.FullPath, "*", SearchOption.AllDirectories))
                {
                    if (Path.GetFileName(file) == "meta.ini") continue;

                    var sha = Utils.FileSHA256(file);

                    ArchiveEntry match;


                    if (indexed.TryGetValue(sha, out var entries))
                    {
                        var pair = (from entry in entries
                                    where entry.archive == primary_source
                                    select entry).FirstOrDefault();

                        if (pair.archive == null)
                        {
                            pair = entries.First();
                        }

                        var install_plan = compiled_mod.InstallPlans.FirstOrDefault(v => v.SourceArchive.SHA256 == pair.archive.SHA256);
                        if (install_plan == null)
                        {
                            install_plan = new InstallPlan();
                            compiled_mod.InstallPlans.Add(install_plan);

                            install_plan.SourceArchive = new InstallSourceArchive();
                            install_plan.FilePairings = new List<FilePairing>();
                            Utils.MemberwiseCopy(pair.archive, install_plan.SourceArchive);

                        }
                        install_plan.FilePairings.Add(new FilePairing()
                        {
                            From = pair.file.FileName,
                            To = Utils.StripPrefix(mod.FullPath, file)
                        });

                    }
                    else
                    {
                        Log.Warn("No match: {0}", file);
                    }

                }
            }
            Log.Info("Done Compiling {0}", mod.ModName);
            return compiled_mod;
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
                Utils.SpitJsonInto(zip, "pack.auto_definition", master);

                foreach (var mod in CompiledMods)
                {
                    Log.Info("Exporting {0}", mod.Name);
                    Utils.SpitJsonInto(zip, Path.Combine("mods", mod.Name, "compiled.json"), mod);
                }
                
            }
        }


    }
}
