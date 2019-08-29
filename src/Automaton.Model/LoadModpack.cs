using Autofac;
using Automaton.Common;
using Automaton.Common.Model;
using Automaton.Model.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Automaton.Model
{
    public class LoadModpack : ILoadModpack
    {
        private readonly IComponentContext _components;

        private readonly IArchiveHandle _archiveHandle;
        private readonly ILifetimeData _lifetimeData;

        public LoadModpack(IComponentContext components)
        {
            _components = components;

            _archiveHandle = components.Resolve<IArchiveHandle>();
            _lifetimeData = components.Resolve<ILifetimeData>();
        }

        public async Task LoadAsync(string modpackPath)
        {
            await Task.Run(() => Load(modpackPath));
        }

        public void Load(string modpackPath)
        {
            var archiveHandle = _archiveHandle.New(modpackPath);
            var archiveEntries = archiveHandle.GetContents().Where(x => !x.IsFolder).ToList();

            var entryMDefinition = archiveEntries.First(x => x.FileName.ToLower() == ConfigPathOffsets.PackDefinitionConfig);

            // Start our prevalidation testing 
            if (entryMDefinition == null)
            {
                return;
            }

            var mDefinition = Utils.LoadJson<MasterDefinition>(entryMDefinition);

            var modEntries = archiveEntries.Where(x => x.FileName.ToLower().Contains("mods\\"))
                .Where(x => x.FileName.EndsWith(ConfigPathOffsets.InstallConfig)).ToList();
            var mods = new List<Mod>();

            foreach (var entry in modEntries)
            {
                mods.Add(Utils.LoadJson<Mod>(entry));
            }

            _lifetimeData.MasterDefinition = mDefinition;
            _lifetimeData.Mods = mods;

            // We also want to grab the fileStreams for required metadata. This includes items like images, ini files and config files. 
            var contentEntries = archiveEntries.Where(x => x.FileName.ToLower().Contains(ConfigPathOffsets.DefaultContentDir)).ToList();

            if (contentEntries.Any())
            {
                var contentItems = contentEntries.Select(x => new ModpackItem()
                {
                    Stream = Utils.GetEntryMemoryStream(x),
                    Name = Path.GetFileName(x.FileName)
                }).ToList();

                _lifetimeData.ModpackContent = contentItems;
            }

            // We want to flip the mods objects so that they're archive-first
            var archives = new List<ExtendedArchive>();

            var patches = archiveHandle.GetContents()
                                       .Where(x => x.FileName.StartsWith("patches\\"))
                                       .ToDictionary(x => Path.GetFileName(x.FileName));

            var managerEntry = archiveEntries.Find(x => Path.GetFileName(x.FileName) == ConfigPathOffsets.ManagerConfig);
            _lifetimeData.ManagerDefinition = Utils.LoadJson<Manager>(managerEntry);

            foreach (var mod in mods)
            {
                var archive = mod.InstallPlans
                    .Select(x => ClassExtensions.ToDerived<SourceArchive, ExtendedArchive>(x.SourceArchive)) // Convert to Extended
                    .Select(x => x.Initialize(_components, mod, patches)) // Initialize each
                    .ToList();

                archives.AddRange(archive);
            }

            // Last cycle through, we need to consolidate the archive objects
            foreach (var archive in archives)
            {
                if (archive.IsValidationComplete)
                {
                    continue;
                }

                // Agg a list of somewhat matching archive objects
                var somewhatMatching = archives.Where(x => x.ModId == archive.ModId && x.FileId == archive.FileId && x != archive).ToList();

                foreach (var somewhatMatch in somewhatMatching) // Bad design for nest
                {
                    somewhatMatch.BindToOther(archive);
                }
            }

            _lifetimeData.Archives = archives;
        }
    }
}
