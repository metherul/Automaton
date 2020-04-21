using System.Collections.Generic;

namespace Gearbox.Shared.ModOrganizer
{
    public class ArchiveSearchOption
    {
        public bool SearchSubDirectories = false;
        public bool SearchUserDirectores = false;
        public bool IgnoreDuplicates = true;
        public bool InfectiousDirectorySearch = true;
        public List<string> AdditionalDirectories = new List<string>();
    }
}