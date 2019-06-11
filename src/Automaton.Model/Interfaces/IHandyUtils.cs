namespace Automaton.Model.Interfaces
{
    public interface IHandyUtils : IModel
    {
        string GetMd5FromFile(string filePath);
    }
}
