using System.Threading.Tasks;

namespace Gearbox.Indexing.Interfaces
{
    public interface IIndexHeader
    {
        string Name { get; set; }
        string RawPath { get; set; }
        IIndexEntry[] IndexEntries { get; set; }

        Task Build(string dir);
    }
}
