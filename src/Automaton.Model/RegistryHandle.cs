using Automaton.Model.Interfaces;
using Microsoft.Win32;

namespace Automaton.Model
{
    public class RegistryHandle : IRegistryHandle
    {
        public string GetGamePath(string gameName)
        {
            var keyPath = $"SOFTWARE\\WOW6432Node\\bethesda softworks\\{gameName}";
            var key = Registry.LocalMachine.OpenSubKey(keyPath);

            if (key == null)
            {
                key = Registry.LocalMachine.OpenSubKey(keyPath);
            }

            if (key == null)
            {
                return string.Empty;
            }

            return key.GetValue("installed path").ToString();
        }
    }
}
