using Microsoft.Win32;
using System;
using System.Threading.Tasks;

namespace Automaton.Model.NexusApi
{
    public class NexusProtocol : Instance.Automaton
    {
        private static string TargetRegistryValue = "";

        public static void SetRegistryValues()
        {
            var targetRegistryPath = @"Computer\HKEY_CURRENT_USER\Software\Classes\nxm\shell\open\command";
            PreviousRegistryValue = Registry.CurrentUser.OpenSubKey(targetRegistryPath).GetValue("").ToString();



        }

        public static void ResetRegistryValues()
        {

        }

        public static async Task CaptureProtocolValues(IProgress<CaptureProtocolValuesProgress> progress)
        {

        }
    }

    public class CaptureProtocolValuesProgress
    {
        public string ModId;
        public string FileId;
    }
}
