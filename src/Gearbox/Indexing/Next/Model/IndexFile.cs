using System;

namespace Gearbox.Indexing.Next.Model
{
    public class IndexFile
    {
        public string Name;
        public string Path;
        public string SHA;
        public string CRC;
        public long Length;
        public DateTime LastModified;
    }
}