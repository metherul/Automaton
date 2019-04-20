using Automaton.Model.Interfaces;

namespace Automaton.Model.NexusApi.Interfaces
{
    public interface IDownloadResponse : IModel
    {
        int Percentage { get; set; }
        string DownloadPath { get; set; }
        DownloadStatus DownloadStatus { get; set; }

        IDownloadResponse New();
    }
}