using System.Collections.Generic;

namespace Gearbox.Shared.ArchiveHandle
{
    public class ExtractOptions
    {
        public List<string> ExtractOnly = new List<string>();
        public bool MaintainStructure = true;
        public bool ReduceMemoryUsage = false;
    }
}