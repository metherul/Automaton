using System;

namespace Hephaestus
{
    class Program
    {
        static void Main(string[] args)
        {
            PackBuilder pb = new PackBuilder();
            pb.LoadPackDefinition("C:\\Mod Organizer 2 - Lexys LOTD SE\\lexy.auto_definition");
            Console.WriteLine("Loaded Definition for {0} by {1}", 
                pb.ModPackMasterDefinition.PackName, 
                pb.ModPackMasterDefinition.AuthorName);

            pb.LoadInstalledMods();
            pb.FindArchives();

            
        }
    }
}
