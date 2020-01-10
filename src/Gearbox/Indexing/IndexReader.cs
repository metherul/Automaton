using Gearbox.Indexing.Interfaces;
using Gearbox.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gearbox.Indexing
{
    public class IndexReader
    {
        private readonly IndexBase _indexBase;

        public IndexReader(IndexBase indexBase)
        {
            _indexBase = indexBase;
        }

        /// <summary>
        /// Returns the contents of the mod index.
        /// </summary>
        /// <returns></returns>
        public async Task<ICollection<IIndexHeader>> GetModsIndex()
        {
            return await GetIndex<ModHeader>(_indexBase.ModsIndexDir);
        }

        /// <summary>
        /// Returns the contents of the archive index.
        /// </summary>
        /// <returns></returns>
        public async Task<ICollection<IIndexHeader>> GetArchiveIndex()
        {
            return await GetIndex<ArchiveHeader>(_indexBase.ArchiveIndexDir);
        }

        /// <summary>
        /// Returns the contents of the game dir index.
        /// </summary>
        /// <returns></returns>
        public async Task<IIndexHeader> GetGameDirIndex()
        {
            var thing = await GetIndex<GameDirHeader>(_indexBase.GameDirIndexDir);

            return thing.FirstOrDefault();
        }

        /// <summary>
        /// Returns the contents of the utilities index.
        /// </summary>
        /// <returns></returns>
        public async Task<IIndexHeader> GetUtilitiesIndex()
        {
            var thing = await GetIndex<UtilitiesHeader>(_indexBase.UtilitiesIndexDir);

            return thing.FirstOrDefault();
        }

        private async Task<ICollection<IIndexHeader>> GetIndex<T>(string dir)
        {
            var dirContents = await AsyncFs.GetDirectoryFiles(dir, "*.json");
            var indexItems = new List<IIndexHeader>();

            foreach (var file in dirContents) 
            {
                indexItems.Add(await JsonUtils.ReadJson<T>(file) as IIndexHeader);
            }

            return indexItems;
        }
    }
}
