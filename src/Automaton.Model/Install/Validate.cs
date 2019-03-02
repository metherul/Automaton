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
            var missingMods = new List<Mod>();
            var directoryContents = directoriesToScan
                .Select(x => Directory.GetFiles(x, "*.*", SearchOption.TopDirectoryOnly)).ToList();

            foreach (var mod in _installBase.ModpackMods)
            {
                var possibleArchiveMatches =
                    directoriesToScan.Where(x => new FileInfo(x).Length.ToString() == mod.FileSize).ToList();

                if (!possibleArchiveMatches.Any())
                {
                    missingMods.Add(mod);
                    continue;
                }

                if (possibleArchiveMatches.Count() == 1)
                {
                    mod.FilePath = possibleArchiveMatches.First();
                    continue;
                }

                if (possibleArchiveMatches.Count() > 1)
                {

                }
            }

            return missingMods;
        }
    }
}
