using System;
using System.Collections.Generic;
using System.Text;

namespace Gearbox.Indexing
{
    public partial class IndexWriter
    {
        public Action<IndexModInfo> IndexModCallback;
        public Action<IndexArchiveInfo> IndexArchiveCallback;

        public class IndexModInfo
        {
            public string ModName;
            public IndexModAction CurrentAction;
        }

        public class IndexArchiveInfo
        {
            public string ArchiveName;
            public IndexArchiveAction CurrentAction;
        }

        public enum IndexModAction
        {
            Analysing,
            Done
        }

        public enum IndexArchiveAction
        {
            Extracting,
            Analysing,
            Done
        }
    }
}
