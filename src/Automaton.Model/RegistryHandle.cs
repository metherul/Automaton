using Automaton.Model.Interfaces;
using Microsoft.Win32;

namespace Automaton.Model
{
    public class RegistryHandle : IRegistryHandle
    {
        private string _keyName;
        private string _valueName;

        public IRegistryHandle New(string keyName, string valueName)
        {
            _keyName = keyName;
            _valueName = valueName;

            return this;
        }

        public void ClearValue()
        {
            SetValue(null);
        }

        public void DeleteKey()
        {
        }

        public void SetValue(object value)
        {
            Registry.SetValue(_keyName, _valueName, value);
        }

        public object GetValue()
        {
            return Registry.GetValue(_keyName, _valueName, null);
        }
    }
}
