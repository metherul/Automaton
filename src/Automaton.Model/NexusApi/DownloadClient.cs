using Automaton.Model.Install;
using Automaton.Model.Install.Intefaces;
using Automaton.Model.NexusApi.Interfaces;
using System;
using System.Net;
using Alphaleonis.Win32.Filesystem;
using Autofac;
using Automaton.Model.Interfaces;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Automaton.Model.NexusApi
{
    public class DownloadClient : IDownloadClient
    {
        private readonly IInstallBase _installBase;
        private readonly ILogger _logger;
        private readonly IApiEndpoints _apiEndpoints;
        private readonly IDownloadResponse _downloadResponse;

        private WebClient _webClient;

        public EventHandler<IDownloadResponse> DownloadUpdate { get; set; }

        public DownloadClient(IComponentContext components)
        {
            _installBase = components.Resolve<IInstallBase>();
            _logger = components.Resolve<ILogger>();
            _apiEndpoints = components.Resolve<IApiEndpoints>();
            _downloadResponse = components.Resolve<IDownloadResponse>();
        }

        public bool DownloadFile(ExtendedMod mod, string downloadUrl)
        {
            _logger.WriteLine($"Downloading file: {mod.FileName}");

            var downloadResponse = _downloadResponse.New();
            DownloadUpdate.Invoke(this, downloadResponse);

            if (string.IsNullOrEmpty(downloadUrl))
            {
                downloadUrl = _apiEndpoints.GenerateModDownloadLinkAsync(mod).Result;
            }

            if (string.IsNullOrEmpty(downloadUrl))
            {
                _logger.WriteLine($"Failed to get downloadUrl for mod: {mod.ModName} | {mod.ModName} | {mod.FileName}");

                _downloadResponse.DownloadStatus = DownloadStatus.Failed;
                DownloadUpdate.Invoke(this, downloadResponse);

                return false;
            }
            
            using (_webClient = new WebClient())
            {
                var platformType = Environment.Is64BitOperatingSystem ? "x64" : "x86";
                var headerString = $"Automaton/{Assembly.GetEntryAssembly().GetName().Version} ({Environment.OSVersion.VersionString}; {platformType}) {RuntimeInformation.FrameworkDescription}";
                _webClient.Headers.Add("User-Agent", headerString);

                var downloadPath = Path.Combine(_installBase.DownloadsDirectory, mod.FileName);

                _webClient.DownloadProgressChanged += (sender, args) =>
                {
                    downloadResponse.Percentage = args.ProgressPercentage;
                    downloadResponse.DownloadStatus = DownloadStatus.Done;

                    DownloadUpdate.Invoke(this, downloadResponse);
                };

                _webClient.DownloadFileCompleted += (sender, args) =>
                {
                    if (args.Cancelled)
                    {

                    }

                    _logger.WriteLine("Download complete");

                    downloadResponse.DownloadStatus = DownloadStatus.Done;
                    DownloadUpdate.Invoke(this, downloadResponse);

                    return;
                };

                try
                {
                    _webClient.DownloadFileAsync(new Uri(downloadUrl), downloadPath);
                }

                catch (Exception e)
                {
                    _logger.WriteLine($"{mod.ModName} could not be downloaded. Exception: {e.Message}", true);

                    downloadResponse.DownloadStatus = DownloadStatus.Failed;
                    DownloadUpdate.Invoke(this, downloadResponse);
                }
            }

            return false;
        }

        public void StopDownload()
        {
            _webClient.CancelAsync();
        }
    }
}
