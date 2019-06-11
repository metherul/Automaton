namespace Automaton.Model.Interfaces
{
    public interface IRegistryHandle : IModel
    {
        IRegistryHandle New(string key);
        void SetValue(object value);
        void ClearValue();
        void DeleteKey();
    }
}
