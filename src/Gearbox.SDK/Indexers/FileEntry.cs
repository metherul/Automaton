using System;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Gearbox.Shared.HashUtils;
using SevenZipExtractor;

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
            return await CreateAsync(file, FileHashType.Md5, string.Empty);
        }

        public static async Task<FileEntry> CreateAsync(string file, FileHashType hashType)
        {
            return await CreateAsync(file, hashType, string.Empty);
        }

        public static async Task<FileEntry> CreateAsync(string file, FileHashType hashType, string relativeTo)
        {
            var fileInfo = new FileInfo(file);
            return new FileEntry()
            {
                Name = fileInfo.Name,
                FilePath = string.IsNullOrEmpty(relativeTo) switch
                {
                    true => Path.GetFullPath(file),
                    false => Path.GetRelativePath(relativeTo, file)
                },
                LastModified = fileInfo.LastWriteTimeUtc,
                Hash = hashType switch
                {
                    FileHashType.Md5 => await FsHash.GetMd5Async(File.OpenRead(file)),
                    FileHashType.Crc32 => (await FsHash.GetCrc32Async(File.OpenRead(file))).ToString(),
                    _ => await FsHash.GetMd5Async(File.OpenRead(file))
                },
                Length = fileInfo.Length
            };
        }

        public static FileEntry Create(Entry archiveEntry)
        {
            var fileEntry = new FileEntry()
            {
                Name = Path.GetFileName(archiveEntry.FileName),
                FilePath = archiveEntry.FileName,
                Length = (long)archiveEntry.Size,
                LastModified = archiveEntry.LastWriteTime.ToUniversalTime(),
                Hash = Convert.ToUInt32(archiveEntry.CRC).ToString()
            };

            return fileEntry;
        }
     }
}
