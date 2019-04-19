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

            //SourceArchives = from_mods.ToList();

        }

        public void LoadPackDefinition(string path)
        {
            ModPackMasterDefinition = Utils.LoadJson<ModPackMasterDefinition>(path);
        }

        internal void LoadInstalledMods()
        {
            InstalledMods = Directory.EnumerateDirectories(ModsFolder)
                            .AsParallel()
                            .Select(mod => InstalledMod.FromFolder(mod))
                            .ToList();
                            
        }
    }
}
