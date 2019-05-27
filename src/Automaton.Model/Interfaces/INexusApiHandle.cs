namespace Automaton.Model.Interfaces
{
    public interface INexusApiHandle
    {
        INexusApiHandle New(string apiKey);
        string GetDownloadLink(object extendedArchive);
    }
}
