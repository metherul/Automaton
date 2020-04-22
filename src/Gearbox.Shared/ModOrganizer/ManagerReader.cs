using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Gearbox.Shared.FsExtensions;
using IniParser;
using Syroot.Windows.IO;

namespace Gearbox.Shared.ModOrganizer
{
    public class ManagerReader
    {
        private readonly string _rootDir;
        private readonly string _modDir;
        private readonly string _profileDir;
        private readonly string _modOrganizerIni;

        public ManagerReader(string exePath)
        {
            _rootDir = new FileInfo(exePath).DirectoryName;
            _modDir = Path.Combine(_rootDir, "mods");
            _profileDir = Path.Combine(_rootDir, "profiles");
            _modOrganizerIni = Path.Combine(_rootDir, "ModOrganizer.ini");
        }

        public Task<string[]> GetModDirs()
        {
            return DirectoryExt.GetDirectoriesAsync(_modDir);
        }

        /// <summary>
        /// Searches various directories for all valid source archive files.
        /// </summary>
        /// <param name="searchOption">Additional search options.</param>
        /// <returns>A of all found archive paths.</returns>
        public async Task<List<string>> FindSourceArchives(ArchiveSearchOption searchOption = null)
        {
            if (searchOption == null)
            {
                searchOption = new ArchiveSearchOption();
            }

            var foundDirs = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            var foundArchives = new List<string>();
            var foundArchiveNames = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

            // Grab all mentioned directories in ModOrganizer.ini
            var iniReader = new IniReader(_modOrganizerIni);
            foundDirs.UnionWith(await iniReader.GetRecentDirs());

            // Grab user directories, if prompted.
            if (searchOption.SearchUserDirectores)
            {
                var userDirs = new List<string>()
                {
                    new KnownFolder(KnownFolderType.Downloads).Path,
                    new KnownFolder(KnownFolderType.Documents).Path,
                    new KnownFolder(KnownFolderType.Desktop).Path
                };

                foundDirs.UnionWith(userDirs);
            }

            // Grab Mod Organizer downloads directory.
            var downloadsDirectory = Path.Combine(_rootDir, "downloads");
            if (Directory.Exists(downloadsDirectory))
            {
                foundDirs.Add(downloadsDirectory);
            }

            // Add any manually targeted directories.
            foundDirs.UnionWith(searchOption.AdditionalDirectories);

            // Add source archives targeted in meta.ini directories.
            var parser = new FileIniDataParser();
            var metaArchives = (await DirectoryExt.GetDirectoriesAsync(_modDir))
                .Select(x => Path.Combine(x, "meta.ini"))
                .Where(File.Exists)
                .Select(x => parser.ReadFile(x)["General"]["installationFile"])
                .Where(x => x != null && File.Exists(x))
                .Select(Path.GetFullPath)
                .Where(x => !foundArchiveNames.Contains(Path.GetFileName(x)))
                .ToList();
            foundArchives.AddRange(metaArchives);
            foundArchiveNames.UnionWith(metaArchives.Select(Path.GetFileName));

            // Since this property is set, add the directories of meta.ini archives to be scanned as well.
            if (searchOption.InfectiousDirectorySearch)
            {
                var metaDirs = metaArchives
                    .Select(x => Path.GetDirectoryName(x))
                    .Where(Directory.Exists);
                foundDirs.UnionWith(metaDirs);
            }

            var archiveFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".7z",
                ".rar",
                ".zip",
                ".tar",
                ".lzma"
            };

            foreach (var dir in foundDirs)
            {
                // Grab the contents of each target directory.
                var contents = (await DirectoryExt.GetFilesAsync(dir, "*", searchOption.SearchSubDirectories
                    ? SearchOption.AllDirectories
                    : SearchOption.TopDirectoryOnly))
                    .Where(x => archiveFilter.Contains(Path.GetExtension(x)) && !foundArchiveNames.Contains(Path.GetFileName(x)))
                    .ToList();

                foundArchives.AddRange(contents);
                foundArchiveNames.UnionWith(contents.Select(Path.GetFileName));
            }

            return foundArchives;
        }
    
        /// <summary>
        /// Returns a <see cref="ProfileReader"/> for the target profile name in the Mod Organizer instance.
        /// </summary>
        /// <param name="profileName">The name of the profile to open.</param>
        /// <returns></returns>
        public ProfileReader GetProfile(string profileName)
        {
            var profileDir = Path.Combine(_profileDir, profileName);
            
            if (!Directory.Exists(profileDir))
            {
                throw new Exception($"The profile: {profileName} does not exist.");
            }

            return new ProfileReader(profileDir);
        }

        /// <summary>
        /// Gets the source archive path in the mod meta.ini, if it exists.
        /// </summary>
        /// <param name="modName">The name of the mod as it appears in Mod Organizer.</param>
        /// <returns>The path of the source archive (this path is not guaranteed to exist).</returns>
        public string GetModSourceArchive(string modName)
        {
            var modDir = Path.Combine(_modDir, modName);
            var metaIni = Path.Combine(modDir, "meta.ini");

            if (!File.Exists(metaIni))
            {
                return null;
            }

            var reader = new FileIniDataParser();
            var archivePath = reader.ReadFile(metaIni)["General"]["installationFile"];

            return archivePath;
        }
    }
}
