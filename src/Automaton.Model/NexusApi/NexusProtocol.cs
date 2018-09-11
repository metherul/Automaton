using Automaton.Model.Utility;
using Microsoft.Win32;
using System;

namespace Automaton.Model.NexusApi
{
    public class NexusProtocol : Instance.Automaton
    {
        private const string TargetRegistryPath = @"Software\Classes\nxm\shell\open\command";

        public NexusProtocol()
        {
            SetRegistryValues();
        }

        /// <summary>
        /// Sets the registry key to the NXMWorker and saves the original value.
        /// </summary>
        private void SetRegistryValues()
        {
            // Get the previous registry value, save in the instance
            PreviousRegistryValue = Registry.CurrentUser.OpenSubKey(TargetRegistryPath).GetValue("").ToString();

            // Write the new registry value
            Registry.CurrentUser.OpenSubKey(TargetRegistryPath, RegistryKeyPermissionCheck.ReadWriteSubTree).SetValue("", NexusHandlerRegistryValue);
        }

        /// <summary>
        /// Resets the modified registry key with the original value.
        /// </summary>
        public void ResetRegistryValues()
        {
            // Reset the registry value to what was set previously
            Registry.CurrentUser.OpenSubKey(TargetRegistryPath).SetValue("", PreviousRegistryValue);
        }

        /// <summary>
        /// Recieves messages from the NXMWorker and routes them back to the calling method by IProgress.
        /// </summary>
        /// <param name="progress"></param>
        public void StartRecievingProtocolValues(IProgress<CaptureProtocolValuesProgress> progress)
        {
            var namedPipes = new NamedPipes(new Progress<string>(protocolResponse =>
            {
                var splitProtocolResponse = protocolResponse.Replace("nxm://", "")
                                                            .Split('/');

                if (true || splitProtocolResponse[0].ToLower() == ModpackHeader.TargetGame)
                {
                    var progressObject = new CaptureProtocolValuesProgress()
                    {
                        ModId = splitProtocolResponse[2],
                        FileId = splitProtocolResponse[4]
                    };

                    // Report back to the calling method
                    progress.Report(progressObject);
                }
            }));

            namedPipes.StartServer();
        }
    }

    public class CaptureProtocolValuesProgress
    {
        public string ModId;
        public string FileId;
    }
}
