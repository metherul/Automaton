using Automaton.Model.Extensions;
using Automaton.Model.ModpackBase;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Automaton.Model.Instance.Interfaces;
using Automaton.Model.ModpackBase.Interfaces;
using Automaton.Model.Utility.Interfaces;

namespace Automaton.Model.Utility
{
    public class ValidationUtilities : IValidationUtilities
    {
        private readonly IAutomatonInstance _automatonInstance;

        public delegate void ValidateSourcesUpdate(IMod currentMod, bool isComputeMd5);

        public event ValidateSourcesUpdate ValidateSourcesUpdateEvent;

        // variables to store information required for the UI to update
        private IMod _currentMod;
        private IMod CurrentMod
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

        private bool _isComputeMd5;
        private bool IsComputeMd5
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

        public List<IMod> MissingMods { get; set; } = new List<IMod>();

        public ValidationUtilities(IAutomatonInstance automatonInstance)
        {
            _automatonInstance = automatonInstance;
        }

        public List<string> GetSourceFiles()
        {
            return Directory.GetFiles(_automatonInstance.SourceLocation, "*.*", SearchOption.TopDirectoryOnly).ToList();
        }

        /// <summary>
        /// Will return a list of <see cref="Mod"/> which do not have a matching archive.
        /// Patches any existing <see cref="Mod"/> objects with updated archive paths.
        /// </summary>
        /// <returns></returns>
        public List<IMod> GetMissingMods(List<string> sourceFiles)
        {
            var sourceFileInfos = sourceFiles.Select(x => new FileInfo(x)).ToList();
            var missingModArchives = new List<IMod>();

            if (!sourceFileInfos.NullAndAny())
            {
                // No files have been found in the source path. This means no mod files were able to be found.
                return _automatonInstance.ModpackMods;
            }

            foreach (var mod in _automatonInstance.ModpackMods)
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

        public async Task<List<IMod>> GetMissingModsAsync(List<string> sourceFiles)
        {
            return await Task.Run(() => GetMissingMods(sourceFiles));
        }

        public async Task<bool> IsMatchingModArchive(IMod mod, string archivePath)
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

        public async Task<bool> IsMatchingModArchiveAsync(IMod mod, string archivePath)
        {
            return await Task.Run(() => IsMatchingModArchive(mod, archivePath));
        }
    }
}