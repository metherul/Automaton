using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Automaton.Model.Extensions;
using Automaton.Model.Instances;

namespace Automaton.Model.Modpack
{
    public class ModValidation
    {
        /// <summary>
        /// Will return a list of <see cref="Mod"/> which do not have a matching archive.
        /// Patches any existing <see cref="Mod"/> objects with updated archive paths.
        /// </summary>
        /// <returns></returns>
        public static List<Mod> GetMissingModsAndPatch()
        {
            var sourceModDirectory = ModpackInstance.SourceLocation;
            var sourceDirectoryFiles = Directory.GetFiles(sourceModDirectory);

            var validationFailedMods = new List<Mod>();

            foreach (var mod in ModpackInstance.ModpackMods)
            {
                var matchingFilesBySize =
                    sourceDirectoryFiles.Where(x => new FileInfo(x).Length.ToString() == mod.ModArchiveSize).ToList();

                if (!matchingFilesBySize.ContainsAny())
                {
                    validationFailedMods.Add(mod);

                    continue;
                }

                foreach (var file in matchingFilesBySize)
                {
                    var md5String = "";

                    while (md5String != mod.ArchiveMd5Sum)
                    {
                        using (var fileStream = File.OpenRead(file))
                        {
                            var md5 = MD5.Create();
                            var md5Bytes = md5.ComputeHash(fileStream);


                        }
                    }
                }
            }

            return validationFailedMods;
        }
    }
}

