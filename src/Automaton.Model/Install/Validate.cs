using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
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

        public Task<List<Mod>> GetMissingModsAsync(params string[] directoriesToScan)
        {
            return Task.Factory.StartNew(() => GetMissingMods(directoriesToScan));
        }

        public List<Mod> GetMissingMods(params string[] directoriesToScan)
        {
            var missingMods = new List<Mod>();
            var directoryContents = directoriesToScan
                .SelectMany(x => Directory.GetFiles(x, "*.*", SearchOption.TopDirectoryOnly)).ToList();

            foreach (var mod in _installBase.ModpackMods)
            {
                var possibleArchiveMatches =
                    directoryContents.Where(x => new FileInfo(x).Length.ToString() == mod.FileSize).ToList();

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
                    var matchingArchive = GetMatchingArchive(mod, possibleArchiveMatches);

                    if (matchingArchive != null)
                    {
                        mod.FilePath = matchingArchive;
                    }
                }
            }

            return missingMods;
        }

        private string GetMatchingArchive(Mod mod, List<string> possibleArchiveMatches)
        {
            foreach (var possibleMatch in possibleArchiveMatches)
            {
                using (var md5 = MD5.Create())
                using (var fileStream = File.OpenRead(possibleMatch))
                {
                    if (BitConverter.ToString(md5.ComputeHash(fileStream)).Replace("-", "").ToLowerInvariant() ==
                        mod.Md5)
                    {
                        return possibleMatch;
                    }
                }
            }

            return null;
        }
    }
}
