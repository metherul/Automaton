using Gearbox.Indexing.Interfaces;

namespace Gearbox.Indexing
{
    public class EntryFactory
    {
        public static IIndexEntry Create(string relativeFilePath, string filePath)
        {
            return new IndexEntry(relativeFilePath, filePath);
        }
    }
}
