using System.Collections.Generic;
using System.IO;
using System.Linq;
using Automaton.Model.Extensions;
using Automaton.Model.Handles;
using Automaton.Model.Instances;

namespace Automaton.Model.Modpack
{
    public class ModValidation
    {
        public delegate Mod ValidateSourcesUpdate();
        public event ValidateSourcesUpdate ValidateSourcesUpdateEvent;

        /// <summary>
        /// Will return a list of <see cref="Mod"/> which do not have a matching archive.
        /// Patches any existing <see cref="Mod"/> objects with updated archive paths.
        /// </summary>
        /// <returns></returns>
        public static List<Mod> ValidateSources()
        {
            var sourceDirectory = @"C:\Programming\C#\Automaton\src\Automaton\bin\Debug\Test";
            var sourceFiles = Directory.GetFiles(sourceDirectory).Select(x => new FileInfo(x)).ToList();
            var missingModArchives = new List<Mod>();

            if (!sourceFiles.ContainsAny())
            {
                return null;
            }

            foreach (var mod in ModpackInstance.ModpackMods)
            {
                var potentialLengthMatches = sourceFiles.Where(x => mod.ModArchiveSize == x.Length.ToString()).ToList();

                if (!potentialLengthMatches.ContainsAny())
                {
                    missingModArchives.Add(mod);
                }

                // Just one potential match in the directory, chances are this should be the right file
                else if (potentialLengthMatches.Count() == 1)
                {
                    mod.ModArchivePath = potentialLengthMatches.First().FullName;
                }

                // For than one matching file length was found, we need to checksum the contents to verify
                else
                {
                    foreach (var match in potentialLengthMatches)
                    {
                        var md5Sum = Md5Handle.CalculateMd5(match.FullName);

                        if (md5Sum == mod.ArchiveMd5Sum)
                        {
                            mod.ModArchivePath = match.FullName;

                            break;
                        }
                    }

                    missingModArchives.Add(mod);
                }
            }

            return missingModArchives;
        }
    }
}
