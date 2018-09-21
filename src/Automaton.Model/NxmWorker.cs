using Automaton.Model.Instance;
using Automaton.Model.Utility;
using Microsoft.Win32;
using System.IO;

namespace Automaton.Model
{
    public class NxmWorker
    {
        public static void InitializeNxmWorker()
        {
            var nxmWorkerPath = Path.Combine(AutomatonInstance.TempDirectory, "NXMWorker.exe");

            // Extract the NXM Worker from the application and move to temporary directory
            Resources.WriteResourceToFile("Automaton.Content.Resources.NXMWorker.exe", nxmWorkerPath);

            // Write path to registry
            var targetRegistryPath = @"Software\Classes\nxm\shell\open\command";
            var test = Registry.CurrentUser.OpenSubKey(targetRegistryPath).GetValue("");

            Registry.CurrentUser.OpenSubKey(targetRegistryPath, RegistryKeyPermissionCheck.ReadWriteSubTree).SetValue("", nxmWorkerPath);
        }
    }
}
