using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hephaestus
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "*.auto_definition|*.auto_definition";

            Log.Info("Please Select a definition file");
            ofd.Title = "Select a Definition File";

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                Log.Warn("Well okay then, keep your secrets.");
                return;
            }
            
            PackBuilder pb = new PackBuilder();
            pb.LoadPackDefinition(ofd.FileName);
            pb.LoadMO2Data();
            pb.LoadPrefs(Path.Combine(pb.ModPackMasterDefinition.MO2Directory, "automaton.prefs"));

            Log.Info("Loaded Definition for {0} by {1}", 
                pb.ModPackMasterDefinition.PackName, 
                pb.ModPackMasterDefinition.AuthorName);

            pb.LoadInstalledMods();
            pb.FindArchives();
            pb.CompileMods();
            pb.CompileGameDirectory();
            pb.CompilePatches();
            pb.ExportPack();
            pb.CleanupPatches();
            Log.Info("Mod pack created");
        }
    }
}
