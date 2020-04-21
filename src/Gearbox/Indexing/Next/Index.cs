using System.IO;

namespace Gearbox.Indexing.Next
{
    public class Index
    {
        internal string ManagerPath;
        internal string ManagerDir;
        internal string IndexPath;
        internal string ModsDir;

        public static Index FromManager(string managerPath)
        {
            var managerDir = Path.GetDirectoryName(managerPath);

            return new Index()
            {
                ManagerPath = managerPath,
                ManagerDir = managerDir,
                IndexPath = Path.Combine(managerDir, "index.automaton"),
                ModsDir = Path.Combine(managerDir, "mods")
            };
        }
    }
}
