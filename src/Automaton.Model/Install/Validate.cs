using System;
using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Autofac;
using Automaton.Model.Install.Intefaces;
using Automaton.Model.Interfaces;
using Automaton.Model.Modpack.Base;

namespace Automaton.Model.Install
{
    public class Validate : IValidate
    {
        private readonly IInstallBase _installBase;
        private readonly ILogger _logger;

        public Validate(IComponentContext components)
        {
            _installBase = components.Resolve<IInstallBase>();
            _logger = components.Resolve<ILogger>();
        }

        public async Task<List<ExtendedMod>> GetMissingModsAsync(List<string> directoriesToScan)
        {
            return await Task.Factory.StartNew(() => GetMissingMods(directoriesToScan));
        }

        public List<ExtendedMod> GetMissingMods(List<string> directoriesToScan)
        {
            directoriesToScan.Add(_installBase.DownloadsDirectory);

            var missingMods = new List<ExtendedMod>();
            var directoryContents = directoriesToScan
                .SelectMany(x => Directory.GetFiles(x, "*.*", System.IO.SearchOption.TopDirectoryOnly)).ToList();

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

            _logger.WriteLine($"Missing mod count: {missingMods.Count()}");

            return missingMods;
        }

        public List<ExtendedMod> FilterMissingMods(string directoryPath)
        {
            var missingMods = new List<ExtendedMod>();
            var directoryContents = Directory.GetFiles(directoryPath, "*.*", System.IO.SearchOption.TopDirectoryOnly).ToList();

            foreach (var mod in _installBase.ModpackMods.Where(x => !File.Exists(x.FilePath)))
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

            _logger.WriteLine($"Missing mod count: {missingMods.Count()}");

            return missingMods;
        }

        public async Task<List<ExtendedMod>> FilterMissingModsAsync(string directoryPath)
        {
            return await Task.Factory.StartNew(() => FilterMissingMods(directoryPath));
        }

        public List<ExtendedMod> ValidateTargetModArchive(string archivePath, List<ExtendedMod> missingMods)
        {
            var archiveSize = new FileInfo(archivePath).Length.ToString();
            var possibleMatchingMods = missingMods.Where(x => x.FileSize == archiveSize).ToList();

            if (!possibleMatchingMods.Any())
            {
                _logger.WriteLine($"Mod archive with archiveSize {archiveSize} had no counterpart in a missingMods collection of size {missingMods.Count}", true);

                return missingMods;
            }

            foreach (var possibleMatchingMod in possibleMatchingMods)
            {
                _installBase.ModpackMods.Where(x => x.FileId == possibleMatchingMod.FileId && x.FileSize == archiveSize).ToList().ForEach(x => x.FilePath = archivePath);

                missingMods.Remove(possibleMatchingMod);
            }

            _logger.WriteLine($"Missing mod count: {missingMods.Count()}");

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
