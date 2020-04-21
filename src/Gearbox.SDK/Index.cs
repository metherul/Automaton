using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Gearbox.Shared.JsonExt;
using Gearbox.SDK.Indexers;

namespace Gearbox.SDK
{
    public static class Indexer
    {
        /// <summary>
        /// Creates a new index for the target Mod Organizer instance.
        /// Note: This will overwrite any pre-existing index.
        /// To check if an index already exists use <see cref="IndexExists"/>.
        /// </summary>
        /// <param name="modOrganizerExe">The path of the Mod Organizer executable./></param>
        /// <returns></returns>
        public static async Task CreateIndex(string modOrganizerExe)
        {
            var modOrganizerDir = Path.GetDirectoryName(modOrganizerExe);
            var archiveIndexFile = Path.Combine(modOrganizerDir, "archives.index");
            var modIndexFile = Path.Combine(modOrganizerDir, "mods.index");

            if (File.Exists(archiveIndexFile))
            {
                File.Delete(archiveIndexFile);
            }

            if (File.Exists(modIndexFile))
            {
                File.Delete(modIndexFile);
            }

            var archiveIndex = new Dictionary<string, ArchiveEntry>();
            var modIndex = new Dictionary<string, ModEntry>();

            await JsonExt.WriteJson(archiveIndex, archiveIndexFile);
            await JsonExt.WriteJson(modIndex, modIndexFile);
        }

        /// <summary>
        /// Checks if an index exists in the target Mod Organizer instance.
        /// </summary>
        /// <param name="modOrganizerExe">The path of the Mod Organizer executable.</param>
        /// <returns>True if the index exists, false if it does not.</returns>
        public static bool IndexExists(string modOrganizerExe)
        {
            var modOrganizerDir = Path.GetDirectoryName(modOrganizerExe);
            var archiveFile = Path.Combine(modOrganizerDir, "archives.index");
            var modFile = Path.Combine(modOrganizerDir, "mods.index");

            return File.Exists(archiveFile) && File.Exists(modFile);
        }

        /// <summary>
        /// Gets the path to the Mod Organizer instance's index.
        /// </summary>
        /// <param name="modOrganizerExe">The path of the Mod Organizer executable.</param>
        /// <returns></returns>
        public static string GetIndexDir(string modOrganizerExe)
        {
            return Path.GetDirectoryName(modOrganizerExe);
        }

        /// <summary>
        /// Opens the target index in read mode.
        /// </summary>
        /// <param name="indexFile">The path of the Mod Organizer instance's index file.</param>
        /// <returns>An <see cref="IndexReader"/> instance.</returns>
        public static async Task<IndexReader> OpenRead(string indexFile)
        {
            var indexReader = new IndexReader();
            await indexReader.LoadIndex(indexFile);

            return indexReader;
        }

        /// <summary>
        /// Opens the target index in write mode.
        /// </summary>
        /// <param name="indexFile">The path of the Mod Organizer instance's index file.</param>
        /// <returns>An <see cref="IndexWriter"/> instance.</returns>
        public static async Task<IndexWriter> OpenWrite(string indexDir)
        {
            var modIndex = Path.Combine(indexDir, "mods.index");
            var archiveIndex = Path.Combine(indexDir, "archives.index");

            var indexWriter = new IndexWriter(modIndex, archiveIndex);
            await indexWriter.Load();

            return indexWriter;
        }
    }
}
