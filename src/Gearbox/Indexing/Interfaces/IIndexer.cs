using System.Threading.Tasks;

namespace Gearbox.Indexing.Interfaces
{
    public interface IIndexer
    {
        public Task<IIndexHeader> Index();
    }
}
