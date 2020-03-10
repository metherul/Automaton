using System.Threading.Tasks;
using Gearbox.Indexing.Factories;
using Gearbox.Indexing.Interfaces;

namespace Gearbox.Indexing.Indexers
{
    public class ModIndexer : IIndexer
    {
        private readonly Index _indexBase;
        private readonly string _path;

        public ModIndexer(Index indexBase, string path)
        {
            _indexBase = indexBase;
            _path = path;
        }

        public async Task<IIndexHeader> Index()
        {
            var header = HeaderFactory.Create(_path);
            await header.Build(_path);

            return header;
        }
    }
}
