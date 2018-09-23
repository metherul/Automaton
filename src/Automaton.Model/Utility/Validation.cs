using Automaton.Model.Extensions;
using Automaton.Model.ModpackBase;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Automaton.Model.Utility
{
    public class Validation
    {
        public delegate void ValidateSourcesUpdate(Mod currentMod, bool isComputeMd5);

        public static event ValidateSourcesUpdate ValidateSourcesUpdateEvent;

        // Static variables to store information required for the UI to update
        private static Mod _currentMod;

        private static Mod CurrentMod
        {
            get => _currentMod;
            set
            {
                if (value != _currentMod)
                {
                    _currentMod = value;

                    ValidateSourcesUpdateEvent(_currentMod, _isComputeMd5);
                }
            }
        }

        private static bool _isComputeMd5;

        private static bool IsComputeMd5
        {
            get => _isComputeMd5;
            set
            {
                if (value != _isComputeMd5)
                {
                    _isComputeMd5 = value;

                    ValidateSourcesUpdateEvent(_currentMod, _isComputeMd5);
                }
            }
        }

        public static List<Mod> MissingMods { get; set; } = new List<Mod>();

        public static List<string> GetSourceFiles()
        {
            return Directory.GetFiles(Instance.AutomatonInstance.SourceLocation, "*.*", SearchOption.TopDirectoryOnly).ToList();
        }

        /// <summary>
        /// Will return a list of <see cref="Mod"/> which do not have a matching archive.
        /// Patches any existing <see cref="Mod"/> objects with updated archive paths.
        /// </summary>
        /// <returns></returns>
        public static List<Mod> GetMissingMods(List<string> sourceFiles)
        {
            var sourceFileInfos = sourceFiles.Select(x => new FileInfo(x)).ToList();
            var missingModArchives = new List<Mod>();

            if (!sourceFileInfos.NullAndAny())
            {
                // No files have been found in the source path. This means no mod files were able to be found.
                return Instance.AutomatonInstance.ModpackMods;
            }

            foreach (var mod in Instance.AutomatonInstance.ModpackMods)
            {
                CurrentMod = mod;

                var potentialLengthMatches = sourceFileInfos.Where(x => mod.FileSize == x.Length.ToString()).ToList();

                if (!potentialLengthMatches.NullAndAny())
                {
                    missingModArchives.Add(mod);
                }

                // Just one potential match in the directory, chances are this should be the right file
                else if (potentialLengthMatches.Count() == 1)
                {
                    mod.FilePath = potentialLengthMatches.First().FullName;
                }

                // For than one matching file length was found, we need to checksum the contents to verify
                else
                {
                    foreach (var match in potentialLengthMatches)
                    {
                        IsComputeMd5 = true;

                        var md5Sum = Md5.CalculateMd5(match.FullName);

                        if (md5Sum == mod.Md5)
                        {
                            mod.FilePath = match.FullName;

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

            return missingModArchives;
        }

        public static async Task<List<Mod>> GetMissingModsAsync(List<string> sourceFiles)
        {
            return await Task.Run(() => GetMissingMods(sourceFiles));
        }

        public static async Task<bool> IsMatchingModArchive(Mod mod, string archivePath)
        {
            var targetMd5 = mod.Md5.ToUpperInvariant();
            var md5Sum = Md5.CalculateMd5(archivePath);

            var matchResult = md5Sum == targetMd5;

            if (matchResult)
            {
                mod.FilePath = archivePath;
            }

            return (matchResult);
        }

        public static async Task<bool> IsMatchingModArchiveAsync(Mod mod, string archivePath)
        {
            return await Task.Run(() => IsMatchingModArchive(mod, archivePath));
        }
    }
}