using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Gearbox.Shared.HashUtils;

namespace Gearbox.SDK.Indexers
{
    public class FileEntry
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public DateTime LastModified { get; set; }
        public string Hash { get; set; }
        public long Length { get; set; }

        public FileEntry()
        {

        }

        public static async Task<FileEntry> CreateAsync(string file)
        {
            var fileInfo = new FileInfo(file);
            var fileEntry = new FileEntry()
            {
                Name = fileInfo.Name,
                FilePath = Path.GetFullPath(file),
                LastModified = fileInfo.LastWriteTimeUtc,
                Hash = await FsHash.GetMd5Async(File.OpenRead(file)),
                Length = fileInfo.Length
            };

            return fileEntry;
        }

        public static async Task<FileEntry> CreateAsync(string file, string relativeTo)
        {
            var fileEntry = await CreateAsync(file);
            fileEntry.FilePath = Path.GetRelativePath(relativeTo, file);

            return fileEntry;
        }
    }
}
