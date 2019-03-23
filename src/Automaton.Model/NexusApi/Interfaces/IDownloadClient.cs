using System;
using Automaton.Model.Install;
using Automaton.Model.Interfaces;

namespace Automaton.Model.NexusApi.Interfaces
{
    public interface IDownloadClient : IService
    {
        EventHandler<ExtendedMod> DownloadUpdate { get; set; }

        bool DownloadFile(string downloadUrl, ExtendedMod mod);
        void QueueDownload(string download, ExtendedMod mod);
    }
}