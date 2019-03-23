namespace Automaton.Model.Interfaces
{
    public interface ICommonFilesystemUtility : IModel
    {
        void DeleteDirectory(string path);
    }
}