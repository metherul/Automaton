namespace Automaton.Model.Interfaces
{
    public interface IRegistryHandle
    {
        IRegistryHandle New(string key);
        void SetValue(object value);
        void ClearValue();
        void DeleteKey();
    }
}
