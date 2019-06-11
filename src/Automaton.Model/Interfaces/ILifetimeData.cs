namespace Automaton.Model.Interfaces
{
    public interface ILifetimeData : ISingleton
    {
        string RequestHeader { get; set; }
        string InstallPath { get; set; }
        string DownloadPath { get; set; }
        string ApiKey { get; set; }
    }
}
