using System;
using System.Threading.Tasks;

namespace Gearbox.Indexing.Interfaces
{
    public interface IIndexEntry
    {
        string RelativeFilePath { get; set; }
        string Hash { get; set; }
        long Length { get; set; }
        DateTime LastModified { get; set; }

        Task Build();
    }
}
