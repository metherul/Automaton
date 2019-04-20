using Automaton.Model.NexusApi.Interfaces;

namespace Automaton.Model.NexusApi
{
    public class DownloadResponse : IDownloadResponse
    {
        public DownloadStatus DownloadStatus { get; set; }

        public string DownloadPath { get; set; }
        public int Percentage { get; set; }

        public IDownloadResponse New()
        {
            DownloadStatus = DownloadStatus.Downloading;

            return this;
        }
    }

    public enum DownloadStatus
    {
        Downloading,
        Failed,
        Done
    }
}
