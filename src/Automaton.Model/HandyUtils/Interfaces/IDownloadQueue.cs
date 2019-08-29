using Automaton.Model.Interfaces;

namespace Automaton.Model.HandyUtils.Interfaces
{
    public interface IDownloadQueue : ISingleton
    {
        void ClearQueue();
        void Enqueue(ExtendedArchive archive);
        void KillController();
        void QueueController();
    }
}