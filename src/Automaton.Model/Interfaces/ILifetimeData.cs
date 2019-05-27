namespace Automaton.Model.Interfaces
{
    public interface ILifetimeData
    {
        string RequestHeader { get; set; }
        string InstallPath { get; set; }
        string DownloadPath { get; set; }
        string ApiKey { get; set; }
    }
}
