using Gearbox.Indexing.Indexers;
using Gearbox.Indexing.Interfaces;
using System.IO;

namespace Gearbox.Indexing.Factories
{
    public class IndexerFactory
    {
        public static IIndexer Create(Index indexBase, string path)
        {
            return File.Exists(path) switch
            {
                true => new ArchiveIndexer(indexBase, path),
                false => new ModIndexer(indexBase, path)
            };
        }
    }
}
