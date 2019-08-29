using Automaton.Model.HandyUtils.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Automaton.Model.HandyUtils
{
    public class DownloadQueue : IDownloadQueue
    {
        private Queue<ExtendedArchive> _queue = new Queue<ExtendedArchive>();

        private List<ExtendedArchive> _downloadingItems = new List<ExtendedArchive>();

        private Thread _queueControllerThread;

        private bool _isControllerRunning;

        public void Enqueue(ExtendedArchive archive)
        {
            if (!_isControllerRunning)
            {
                _isControllerRunning = true;

                _queueControllerThread = new Thread(() => QueueController());
                _queueControllerThread.Start();
            }

            if (!archive.IsValidationComplete)
            {
                _queue.Enqueue(archive);
            }
        }

        public void ClearQueue()
        {
            _queue.Clear();
        }

        public void KillController()
        {
            _isControllerRunning = false;
            _queueControllerThread.Abort();

            _queue.Clear();
        }

        public void QueueController()
        {
            while (_isControllerRunning)
            {

                _downloadingItems = _downloadingItems.ToList().Where(x => x.IsDownloading == false || !x.IsValidationComplete).ToList();

                if (_queue.Any() && _downloadingItems.Count() <= 5)
                {
                    var archive = _queue.Dequeue();
                    archive.DownloadThreaded();

                    _downloadingItems.Add(archive);
                }

                else
                {

                }

                Thread.Sleep(100);
            }
        }
    }
}
