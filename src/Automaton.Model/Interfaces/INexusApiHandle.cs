namespace Automaton.Model.Interfaces
{
    public interface INexusApiHandle : IModel
    {
        INexusApiHandle New(string apiKey);
        string GetDownloadLink(object extendedArchive);
    }
}
