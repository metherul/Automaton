using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gearbox.SDK.Indexers
{
    public class IndexRoot
    {
        public string ModOrganizerPath { get; set; }
        public List<ModEntry> ModEntries { get; set; }
        public List<ArchiveEntry> ArchiveEntries { get; set; }
    }
}
