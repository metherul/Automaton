using Gearbox.SDK.Indexers;
using Gearbox.Shared.ArchiveHandle;
using Gearbox.Shared.FsExtensions;
using Gearbox.Shared.HashUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gearbox.SDK
{
    public class ArchiveEntry
    {
        public string Name { get; set; }
        public string ArchivePath { get; set; }
        public long Length { get; set; }
        public DateTime LastModified { get; set; }
        public string FilesystemHash { get; set; }
        public string Hash { get; set; }
        public List<FileEntry> FileEntries { get; set; }

        public ArchiveEntry()
        {
            
        }

        /// <summary>
        /// Creates an ArchiveEntry from the target archive's path.
        /// Note that this process will:
        /// 1. Extract the archive to a temp folder.
        /// 2. Index all files.
        /// 3. Delete the extracted files.
        /// </summary>
        /// <param name="archivePath">The path of the archive to index.</param>
        /// <returns></returns>
        public static async Task<ArchiveEntry> CreateAsync(string archivePath)
        {
            var archiveInfo = new FileInfo(archivePath);
            var archiveName = archiveInfo.Name;

            // Extract the contents of the archive to a temp directory.
            var extractDir = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(archiveName));
            var archiveHandle = new ArchiveHandle(archivePath);

            // Here we begin extracting the archive and process each file as its decompressed.
            var fileEntries = new List<FileEntry>();
            var entryTasks = new List<Task<FileEntry>>();
            archiveHandle.FileExtractedEvent += (path) =>
            {
                var fileEntry = FileEntry.CreateAsync(path, relativeTo: extractDir);
                entryTasks.Add(fileEntry);
            };
            await archiveHandle.Extract(extractDir);
            await Task.WhenAll(entryTasks);

            var archiveEntry = new ArchiveEntry()
            {
                Name = archiveName,
                ArchivePath = archivePath,
                LastModified = archiveInfo.LastWriteTimeUtc,
                FilesystemHash = await FsHash.MakeFilesystemHash(extractDir),
                Hash = await FsHash.GetMd5Async(File.OpenRead(archivePath)),
                FileEntries = entryTasks.Select(x => x.Result).ToList(),
                Length = archiveInfo.Length
            };

            // Delete the extraction directory.
            await DirectoryExt.DeleteAsync(extractDir);

            return archiveEntry;
        }

        public static async Task<ArchiveEntry> CreateFastAsync(string archivePath)
        {
            var archiveEntries = new ArchiveHandle(archivePath).GetArchiveEntries();
            var archiveInfo = new FileInfo(archivePath);
            var archiveEntry = new ArchiveEntry()
            {
                Name = archiveInfo.Name,
                ArchivePath = archivePath,
                LastModified = archiveInfo.LastWriteTimeUtc,
                Hash = await FsHash.GetMd5Async(File.OpenRead(archivePath)),
                FileEntries = archiveEntries.Select(x => new FileEntry()
                {
                    FilePath = x.FileName,
                    Length = (long)x.Size,
                    Hash = Convert.ToUInt32(x.CRC).ToString()
                }).ToList(),
                Length = archiveInfo.Length
            };

            return archiveEntry;
        }
    }
}
