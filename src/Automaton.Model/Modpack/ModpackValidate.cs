using System.Collections.Generic;
using System.Linq;
using Autofac;
using Automaton.Model.Interfaces;
using Automaton.Model.Modpack.Base.Interfaces;
using Automaton.Model.Modpack.Interfaces;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;

namespace Automaton.Model.Modpack
{
    public class ModpackValidate : IModpackValidate
    {
        private readonly IModpackStructure _modpackStructure;
        private readonly ILogger _logger;

        public ModpackValidate(IComponentContext components)
        {
            _modpackStructure = components.Resolve<IModpackStructure>();
            _logger = components.Resolve<ILogger>();
        }

        /// <summary>
        /// Stub. Needs to be implemented, but not imperatively important.
        /// </summary>
        /// <param name="modpackEntries"></param>
        /// <returns></returns>
        public bool ValidateCorrectModpackStructure(List<IArchiveEntry> modpackEntries)
        {
            if (modpackEntries.Any(x => x.Key == ConfigPathOffsets.PackDefinitionConfig))
            {
                return true;
            }

            _logger.WriteLine($"This archive is not a valid modpack. Missing '{ConfigPathOffsets.PackDefinitionConfig}'.", true);

            return false;
        }
    }
}
