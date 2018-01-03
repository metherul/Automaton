using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Automaton.Model
{
    class PackHandlerHelper
    {
        /// <summary>
        /// Gets a list of source file infos from the mod source location.
        /// </summary>
        /// <param name="mods">List of mods</param>
        /// <param name="sourceLocation">Mod source location</param>
        /// <returns></returns>
        public static List<FileInfo> GetSourceFiles(List<Mod> mods, string sourceLocation)
        {
            var sourceFiles = Directory.GetFiles(sourceLocation);
            var sourceFileInfos = sourceFiles.Select(x => new FileInfo(x));
            var matchingSourceFiles = new List<FileInfo>();

            foreach (var mod in mods)
            {
                var matchingFiles = sourceFileInfos.Where(x => x.Length.ToString() == mod.FileSize || x.Name == mod.FileName);

                if (matchingFiles.Count() == 1)
                {
                    matchingSourceFiles.Add(matchingFiles.First());
                }
            }

            return matchingSourceFiles;
        }

        /// <summary>
        /// Checks the FlagHandler for any matching conditional values.
        /// </summary>
        /// <param name="conditional"></param>
        /// <returns></returns>
        public static bool ShouldRemoveInstallation(Conditional conditional)
        {
            var matchingValues = FlagHandler.FlagList.Where(x => x.FlagName == conditional.Name
                && x.FlagValue == conditional.Value);

            // There is a value in matchingValues = return false;
            // No value = return true;
            return !(matchingValues.Count() > 0);
        }

        /// <summary>
        /// Detects if the ModPack contains a non-null Optionals object with values.
        /// </summary>
        /// <param name="modPack"></param>
        /// <returns></returns>
        public static bool DoOptionalsExist(ModPack modPack)
        {
            // Note to self: Clean this up in the future.

            var modPackOptionals = modPack.OptionalInstallation;

            if (modPackOptionals == null || modPackOptionals.Title == null || modPackOptionals.Groups == null)
            {
                return false;
            }

            if (modPackOptionals.Groups.Count == 0)
            {
                return false;
            }

            return true;
        }
    }
}
