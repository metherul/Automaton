using System.Collections.Generic;
using Automaton.Model.Modpack;

namespace Automaton.Model.Install
{
    public class InstallInstance
    {
        public static List<Mod> ModInstallList { get; set; }

        public static string ModpackName { get; set; }

        public static string InstallDirectory { get; set; }
        public static string ModDownloadDirectory { get; set; }

        public static bool InstallModOrganizer { get; set; }
    }
}
