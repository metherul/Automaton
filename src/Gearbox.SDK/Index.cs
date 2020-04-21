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
            var indexFile = Path.Combine(modOrganizerDir, "index.automaton");

            if (File.Exists(indexFile))
            {
                File.Delete(indexFile);
            }

            var indexRoot = new IndexRoot()
            {
                ModOrganizerPath = modOrganizerExe,
                ModEntries = new List<ModEntry>(),
                ArchiveEntries = new List<ArchiveEntry>()
            };

            await JsonExt.WriteJson(indexRoot, indexFile);
        }

        /// <summary>
        /// Checks if an index exists in the target Mod Organizer instance.
        /// </summary>
        /// <param name="modOrganizerExe">The path of the Mod Organizer executable.</param>
        /// <returns>True if the index exists, false if it does not.</returns>
        public static bool IndexExists(string modOrganizerExe)
        {
            return File.Exists(Path.Combine(Path.GetDirectoryName(modOrganizerExe), "index.automaton"));
        }

        /// <summary>
        /// Gets the path to the Mod Organizer instance's index.
        /// </summary>
        /// <param name="modOrganizerExe">The path of the Mod Organizer executable.</param>
        /// <returns></returns>
        public static string GetIndexFile(string modOrganizerExe)
        {
            return Path.Combine(Path.GetDirectoryName(modOrganizerExe), "index.automaton");
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
        public static IndexWriter OpenWrite(string indexFile)
        {
            return new IndexWriter(indexFile);
        }
    }
}
