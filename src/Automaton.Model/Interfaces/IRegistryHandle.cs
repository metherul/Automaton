namespace Automaton.Model.Interfaces
{
    public interface IRegistryHandle : IModel
    {
        void SetValue(object value);
        void ClearValue();
        void DeleteKey();
        IRegistryHandle New(string keyName, string valueName);
        object GetValue();
    }
}
