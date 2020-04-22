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
                var fileEntry = FileEntry.CreateAsync(file, modDir);
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
    
        public static async Task<ModEntry> CreateFastAsync(string modDir)
        {
            var dirInfo = new DirectoryInfo(modDir);

            var contents = await DirectoryExt.GetFilesAsync(modDir, "*", SearchOption.AllDirectories);
            var entryTasks = new List<Task<FileEntry>>();

            foreach (var file in contents)
            {
                var fileEntry = FileEntry.CreateAsync(file, modDir);
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
    }
}
