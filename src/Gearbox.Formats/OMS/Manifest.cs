using System.Collections.Generic;

namespace Gearbox.Formats.OMS
{
    public class Manifest
    {
        public string PackType;
        public string Name;
        public string Version;
        public string Author;
        public string Homepage;
        public string SupportPage;
        public string DiscordInvite;
        public bool HasTheme;

        public bool CanUpdate;
        public UpdateMethod UpdateMethod;

        public List<FeatureSet> FeatureSet;

        public List<RegisterEntry> RegisterTable;
        public List<PatchEntry> PatchTable;
        public List<PatcherEntry> PatcherTable;
    }
}