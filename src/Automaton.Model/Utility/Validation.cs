using System.Collections.Generic;
using System.IO;
using System.Linq;
using Automaton.Model.Extensions;

namespace Automaton.Model.Utility
{
    public class Validation
    {
        public delegate void ValidateSourcesUpdate();
        public static event ValidateSourcesUpdate ValidateSourcesUpdateEvent;

        // Static variables to store information required for the UI to update
        private static Mod _currentMod;
        public static Mod CurrentMod
        {
            get => _currentMod;
            set
            {
                if (value != _currentMod)
                {
                    _currentMod = value;

                    ValidateSourcesUpdateEvent();
                }
            }
        }

        private static bool _isComputeMd5;
        public static bool IsComputeMd5
        {
            get => _isComputeMd5;
            set
            {
                if (value != _isComputeMd5)
                {
                    _isComputeMd5 = value;

                    ValidateSourcesUpdateEvent();
                }
            }
        }

        private static bool _isComplete;
        public static bool IsComplete
        {
            get => _isComputeMd5;
            set
            {
                if (value != _isComputeMd5)
                {
                    _isComputeMd5 = value;

                    ValidateSourcesUpdateEvent();
                }
            }
        }

        /// <summary>
        /// Will return a list of <see cref="Mod"/> which do not have a matching archive.
        /// Patches any existing <see cref="Mod"/> objects with updated archive paths.
        /// </summary>
        /// <returns></returns>
        public static List<Mod> ValidateSources(List<string> sourceFiles)
        {
            var sourceFileInfos = sourceFiles.Select(x => new FileInfo(x)).ToList();
            var missingModArchives = new List<Mod>();

            if (!sourceFileInfos.ContainsAny())
            {
                return null;
            }

            foreach (var mod in Instance.Automaton.ModpackMods)
            {
                CurrentMod = mod;

                var potentialLengthMatches = sourceFileInfos.Where(x => mod.ModArchiveSize == x.Length.ToString()).ToList();

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
                        IsComputeMd5 = true;

                        var md5Sum = MD5.CalculateMd5(match.FullName);

                        if (md5Sum == mod.ArchiveMd5Sum)
                        {
                            mod.ModArchivePath = match.FullName;

                            break;
                        }

                        // Zero matches were found
                        if (potentialLengthMatches.IndexOf(match) == potentialLengthMatches.Count - 1)
                        {
                            missingModArchives.Add(mod);
                        }
                    }

                    IsComputeMd5 = false;
                }
            }

            IsComplete = true;

            return missingModArchives;
        }

        public static List<string> GetSourceFiles()
        {
            var sourceDirectory = @"C:\Programming\C#\Automaton\src\Automaton.View\bin\Debug\Test";

            return Directory.GetFiles(sourceDirectory, "*.*", SearchOption.TopDirectoryOnly).ToList();
        }
    }
}
