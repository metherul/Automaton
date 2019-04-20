using System;
using Automaton.Model.Install;
using Automaton.Model.Interfaces;

namespace Automaton.Model.NexusApi.Interfaces
{
    public interface IDownloadClient : IModel
    {
        EventHandler<IDownloadResponse> DownloadUpdate { get; set; }

        bool DownloadFile(ExtendedMod mod, string downloadUrl = "");
        void StopDownload();
    }
}