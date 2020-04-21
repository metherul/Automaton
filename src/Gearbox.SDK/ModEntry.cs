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
            var modEntry = new ModEntry()
            {
                Name = dirInfo.Name,
                Directory = modDir,
                FilesystemHash = await FsHash.MakeFilesystemHash(modDir)
            };

            var contents = await DirectoryExt.GetFilesAsync(modEntry.Directory, "*", SearchOption.AllDirectories);

            foreach (var file in contents)
            {
                var fileEntry = await FileEntry.CreateAsync(file, modDir);
                modEntry.FileEntries.Add(fileEntry);
            }

            return modEntry;
        }
    }
}
