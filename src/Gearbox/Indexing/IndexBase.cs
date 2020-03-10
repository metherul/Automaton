using System.IO;

namespace Gearbox.Indexing
{
    public class Index
    {
        public readonly string ModOrganizerDir;
        public readonly string ModsDir;

        public readonly string AutomatonDir;
        public readonly string IndexDir;
        public readonly string ExtractDir;
        public readonly string ModsIndexDir;
        public readonly string ArchiveIndexDir;
        public readonly string GameDirIndexDir;
        public readonly string UtilitiesIndexDir;

        public Index(string moExePath)
        {
            ModOrganizerDir = Path.GetDirectoryName(moExePath);
            ModsDir = Path.Combine(ModOrganizerDir, "mods");

            AutomatonDir = Path.Combine(ModOrganizerDir, "automaton");
            IndexDir = Path.Combine(AutomatonDir, "index");
            ExtractDir = Path.Combine(IndexDir, "extract");
            ArchiveIndexDir = Path.Combine(IndexDir, "archives");
            ModsIndexDir = Path.Combine(IndexDir, "mods");
            GameDirIndexDir = Path.Combine(IndexDir, "gamedir");
            UtilitiesIndexDir = Path.Combine(IndexDir, "utilities");

            if (!Directory.Exists(ModsIndexDir))
            {
                Directory.CreateDirectory(ModsIndexDir);
            }

            if (!Directory.Exists(ArchiveIndexDir))
            {
                Directory.CreateDirectory(ArchiveIndexDir);
            }

            if (!Directory.Exists(GameDirIndexDir))
            {
                Directory.CreateDirectory(GameDirIndexDir);
            }

            if (!Directory.Exists(UtilitiesIndexDir))
            {
                Directory.CreateDirectory(UtilitiesIndexDir);
            }
        }
    }
}