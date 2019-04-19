using Automaton.Model.Modpack;
using Automaton.Utils;
using Hephaestus.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Hephaestus.Nexus;

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

    }
}
