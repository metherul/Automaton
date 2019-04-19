using System;
using System.Threading.Tasks;

namespace Hephaestus
{
    class Program
    {
        static void Main(string[] args)
        {
            PackBuilder pb = new PackBuilder();
            pb.LoadPackDefinition("C:\\Mod Organizer 2 - Lexys LOTD SE\\lexy.auto_definition");
            pb.LoadMO2Data();
            pb.LoadPrefs("C:\\Mod Organizer 2 - Lexys LOTD SE\\halgari.prefs");

            Log.Info("Loaded Definition for {0} by {1}", 
                pb.ModPackMasterDefinition.PackName, 
                pb.ModPackMasterDefinition.AuthorName);

            pb.LoadInstalledMods();
            pb.FindArchives();
            pb.CompileMods();

            
        }
    }
}
