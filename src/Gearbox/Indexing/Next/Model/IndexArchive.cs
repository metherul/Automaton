using System.Collections.Generic;

namespace Gearbox.Indexing.Next.Model
{
    public class IndexArchive
    {
        public string Path;
        public long Length;
        public string Md5;
        public IndexFile[] Contents;
    }
}