using Autofac;
using Automaton.Common;
using Automaton.Common.Model;
using Automaton.Model.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Automaton.Model
{
    public class LoadModpack : ILoadModpack
    {
        private readonly IArchiveHandle _archiveHandle;
        private readonly ILifetimeData _lifetimeData;

        public LoadModpack(IComponentContext components)
        {
            _archiveHandle = components.Resolve<IArchiveHandle>();
            _lifetimeData = components.Resolve<ILifetimeData>();
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

            var mDefinitionStream = new MemoryStream();
            entryMDefinition.Extract(mDefinitionStream);

            mDefinitionStream.Seek(0, SeekOrigin.Begin);

            var mDefinition = Utils.LoadJson<MasterDefinition>(mDefinitionStream);

            var modEntries = archiveEntries.Where(x => x.FileName.ToLower().Contains("mods\\"))
                .Where(x => x.FileName.EndsWith(ConfigPathOffsets.InstallConfig)).ToList();
            var mods = new List<Mod>();

            foreach (var entry in modEntries)
            {
                var entryStream = new MemoryStream();
                entry.Extract(entryStream);

                entryStream.Seek(0, SeekOrigin.Begin);

                // This has the chance of spitting out some errors further down the line, will get back to later.
                var modObject = Utils.LoadJson<Mod>(entryStream);

                mods.Add(modObject);
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
        }
    }
}
