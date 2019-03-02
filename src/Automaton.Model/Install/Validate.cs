using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac;
using Automaton.Model.Install.Intefaces;
using Automaton.Model.Modpack.Base;

namespace Automaton.Model.Install
{
    public class Validate : IValidate
    {
        private readonly IInstallBase _installBase;

        public Validate(IComponentContext components)
        {
            _installBase = components.Resolve<IInstallBase>();
        }

        public List<Mod> GetMissingMods(params string[] directoriesToScan)
        {
            var directoryContents = directoriesToScan
                .Select(x => Directory.GetFiles(x, "*.*", SearchOption.TopDirectoryOnly)).ToList();

            

            return null;
        }

        public bool FindMissingMod(Mod mod, List<string> possibleFileMatches)
        {
            return false;
        }
    }
}
