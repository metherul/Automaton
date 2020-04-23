using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Gearbox.SDK.Indexers;
using Gearbox.Shared.FsExtensions;
using Gearbox.Shared.HashUtils;

namespace Gearbox.SDK
{
    public class ModEntry
    {
        public string Name { get; set; }
        public string Directory { get; set; }
        public string FilesystemHash { get; set; }
        public List<FileEntry> FileEntries { get; set; } = new List<FileEntry>();

        public ModEntry()
        {

        }

        public static async Task<ModEntry> CreateAsync(string modDir)
        {
            var dirInfo = new DirectoryInfo(modDir);

            var contents = await DirectoryExt.GetFilesAsync(modDir, "*", SearchOption.AllDirectories);
            var entryTasks = new List<Task<FileEntry>>();

            foreach (var file in contents)
            {
                var fileEntry = FileEntry.CreateAsync(file, FileHashType.Md5, modDir);
                entryTasks.Add(fileEntry);
            }

            var modEntry = new ModEntry()
            {
                Name = dirInfo.Name,
                Directory = modDir,
                FilesystemHash = await FsHash.MakeFilesystemHash(modDir)
            };

            await Task.WhenAll(entryTasks);
            modEntry.FileEntries = entryTasks.Select(x => x.Result).ToList();

            return modEntry;
        }

        /// <summary>
        /// Creates an ModEntry from the target mod dir.
        /// This function uses the quicker CRC32 hash algorithm instead of MD5.
        /// Due to the nature of CRC32, this function should not be considered 100% accurate. 
        /// These innaccuracies are mitigated in <see cref="MatchResultReducer.Reduce(List{MatchResult}, ModEntry, FileEntry, ReduceOptions)"/>
        /// but it cannot be considered to be completely fail-safe.
        /// </summary>
        /// <param name="modDir"></param>
        /// <returns></returns>
        public static async Task<ModEntry> CreateFastAsync(string modDir)
        {
            var dirInfo = new DirectoryInfo(modDir);

            var contents = await DirectoryExt.GetFilesAsync(modDir, "*", SearchOption.AllDirectories);
            var entryTasks = new List<Task<FileEntry>>();
            var fileEntries = new List<FileEntry>();

            foreach (var file in contents)
            {
                var fileEntry = await FileEntry.CreateAsync(file, FileHashType.Crc32, modDir);
                fileEntries.Add(fileEntry);
            }

            var modEntry = new ModEntry()
            {
                Name = dirInfo.Name,
                Directory = modDir,
                FilesystemHash = await FsHash.MakeFilesystemHash(modDir)
            };

            // await Task.WhenAll(entryTasks);
            // modEntry.FileEntries = entryTasks.Select(x => x.Result).ToList();

            return modEntry;
        }
    }
}
