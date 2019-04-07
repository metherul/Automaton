using System;
using Automaton.Model.Install;
using Automaton.Model.Interfaces;

namespace Automaton.Model.NexusApi.Interfaces
{
    public interface IDownloadClient : IService
    {
        EventHandler<(ExtendedMod, bool)> DownloadUpdate { get; set; }
        
        void PurgeQueue();
        void PurgeQueue(ExtendedMod mod);

        bool DownloadFile(ExtendedMod mod, string downloadUrl = "");
        void QueueDownload(ExtendedMod mod, string downloadUrl);
    }
}