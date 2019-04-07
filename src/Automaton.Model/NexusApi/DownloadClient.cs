using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Automaton.Model.Install;
using Automaton.Model.Install.Intefaces;
using Automaton.Model.NexusApi.Interfaces;
using System;
using System.Linq;
using System.Net;
using Alphaleonis.Win32.Filesystem;
using Autofac;
using System.Threading;
using System.Collections.Generic;
using Automaton.Model.Interfaces;

namespace Automaton.Model.NexusApi
{
    public class DownloadClient : IDownloadClient
    {
        private readonly IInstallBase _installBase;
        private readonly ILogger _logger;
        private readonly IApiEndpoints _apiEndpoints;

        private Queue<(ExtendedMod, string)> _downloadQueue = new Queue<(ExtendedMod, string)>();

        private int _currentDownloads;
        private readonly int _maxConcurrentDownloads = 1;

        public EventHandler<(ExtendedMod, bool)> DownloadUpdate { get; set; }

        public DownloadClient(IComponentContext components)
        {
            _installBase = components.Resolve<IInstallBase>();
            _logger = components.Resolve<ILogger>();
            _apiEndpoints = components.Resolve<IApiEndpoints>();

            Task.Factory.StartNew(QueueController);
        }

        public void PurgeQueue()
        {
            _downloadQueue.Clear();
        }

        public void PurgeQueue(ExtendedMod mod)
        {
            _downloadQueue = new Queue<(ExtendedMod, string)>(_downloadQueue.Where(x => x.Item1 != mod).ToList());
        }

        private void QueueController()
        {
            while (true)
            {
                if (_downloadQueue.Any() && _currentDownloads != _maxConcurrentDownloads)
                {
                    var queueObject = _downloadQueue.Dequeue();

                    Task.Factory.StartNew(() => DownloadFile(queueObject.Item1, queueObject.Item2));
                    _currentDownloads++;
                }

                Thread.Sleep(300);
            }
        }

        public void QueueDownload(ExtendedMod mod, string downloadUrl = "")
        {
            if (_downloadQueue.ToList().Any(x => x.Item1.Md5 == mod.Md5))
            {
                return;
            }

            _downloadQueue.Enqueue((mod, downloadUrl));
        }

        public bool DownloadFile(ExtendedMod mod, string downloadUrl)
        {
            _logger.WriteLine($"Downloading file: {mod.FileName}");

            mod.CurrentDownloadProgress = 0;
            mod.IsIndeterminateProcess = false;
            DownloadUpdate.Invoke(this, (mod, false));

            if (string.IsNullOrEmpty(downloadUrl))
            {
                downloadUrl = _apiEndpoints.GenerateModDownloadLinkAsync(mod).Result;
            }

            if (string.IsNullOrEmpty(downloadUrl))
            {
                _logger.WriteLine($"Failed to get downloadUrl for mod: {mod.ModName} | {mod.ModName} | {mod.FileName}");
                _currentDownloads--;

                DownloadUpdate.Invoke(this, (mod, true));

                return false;
            }
            
            using (var webClient = new WebClient())
            {
                var downloadPath = Path.Combine(_installBase.DownloadsDirectory, mod.FileName);

                webClient.DownloadProgressChanged += (sender, args) =>
                {
                    if (mod.CurrentDownloadProgress == 100)
                    {
                        _logger.WriteLine("Download complete");

                        return;
                    }

                    mod.CurrentDownloadProgress = args.ProgressPercentage;
                    DownloadUpdate.Invoke(this, (mod, false));
                };

                webClient.DownloadFileCompleted += (sender, args) =>
                {
                    mod.FilePath = downloadPath;
                    _currentDownloads--;
                };
                
                webClient.DownloadFileAsync(new Uri(downloadUrl), downloadPath);                
            }

            return false;
        }
    }
}
