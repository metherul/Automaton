using System.Collections.Generic;
using Automaton.Model.Modpack.Base.Interfaces;
using Automaton.Model.Modpack.Interfaces;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;

namespace Automaton.Model.Modpack
{
    public class ModpackValidate : IModpackValidate
    {
        private readonly IModpackStructure _modpackStructure;

        public ModpackValidate(IModpackStructure modpackStructure)
        {
            _modpackStructure = modpackStructure;
        }

        /// <summary>
        /// Stub. Needs to be implemented, but not imperatively important.
        /// </summary>
        /// <param name="modpackEntries"></param>
        /// <returns></returns>
        public bool ValidateCorrectModpackStructure(List<IArchiveEntry> modpackEntries)
        {
            return true;
        }
    }
}
