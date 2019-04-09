namespace Automaton.Model.Interfaces
{
    public interface IRegistryHandle : IService
    {
        string GetGamePath(string gameName);
        void ClearMOCurrentInstance();
    }
}