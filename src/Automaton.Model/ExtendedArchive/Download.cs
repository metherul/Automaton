using Alphaleonis.Win32.Filesystem;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Automaton.Model
{
    public partial class ExtendedArchive
    {
        public string AuthenticationParams { get; set; }
        public double MbPerSecond { get; set; }
        public int DownloadPercentage { get; set; }
        public bool IsDownloading { get; set; }

        public async Task DownloadAsync()
        {
            // Check to make sure we're not already downloading this file
            if (IsDownloading || IsValidationComplete)
            {
                return;
            }

            IsDownloading = true;

            string downloadLink = null;
            if (DirectURL != null)
            {
                // Use a direct URL if we have it
                downloadLink = DirectURL;
            }

            if (Repository == null)
            {
                IsDownloading = false;
                _lifetimeData.CurrentDownloads--;
                return;
            }

            if (Repository == "Nexus" && downloadLink == null)
            {
                // We need to grab the download URL for this given item
                downloadLink = await _nexusApi.GetArchiveDownloadUrl(this, AuthenticationParams);

                if (downloadLink == null)
                {
                    IsDownloading = false;
                    _lifetimeData.CurrentDownloads--;
                    _dialogRedirector.RouteLog($"Failed to get Nexus download link for {ArchiveName}. This file must be downloaded manually. If this issue persists, please contact the modpack developer.");

                    return;
                }
            }

            // Initialize the webClient and set required headers
            _webClient = new WebClient();
            _webClient.Headers.Add("User-Agent", _lifetimeData.UserAgent);

            var archivePath = Path.Combine(_lifetimeData.DownloadPath, ArchiveName);
            var partPath = Path.Combine(_lifetimeData.DownloadPath, ArchiveName + ".part");

            if (File.Exists(partPath))
            {
                File.Delete(partPath);
            }

            var lastBytesRecieved = (long)0;
            var dateTime = DateTime.MinValue;

            _webClient.DownloadProgressChanged += async (sender, e) =>
            {
                if (dateTime == DateTime.MinValue)
                {
                    dateTime = DateTime.Now;
                }

                var timeSpan = DateTime.Now - dateTime;
                lastBytesRecieved = e.BytesReceived - lastBytesRecieved;

                var bytesPerSecond = e.BytesReceived / (double)timeSpan.TotalSeconds;

                MbPerSecond = Math.Round((double)bytesPerSecond / ((double)1024 * (double)1024), 2);
                DownloadPercentage = (int)(1000 * ((double)e.BytesReceived / (double)e.TotalBytesToReceive));
            };

            _webClient.DownloadFileCompleted += async (sender, e) =>
            {
                if (e.Cancelled)
                {
                    DownloadPercentage = 0;

                    File.Delete(partPath);
                }

                else
                {
                    if (File.Exists(archivePath))
                    {
                        File.Delete(archivePath);
                    }

                    File.Move(partPath, archivePath);

                    await TryLoadAsync(Path.Combine(_lifetimeData.DownloadPath, ArchiveName));
                }

                _lifetimeData.CurrentDownloads--;
                IsDownloading = false;

                return;
            };

            _webClient.DownloadFileAsync(new Uri(downloadLink), partPath);
        }

        public void DownloadThreaded()
        {
            var thread = new Thread(() => DownloadAsync());
            thread.Start();
        }


        public void CancelDownloadThreaded()
        {
            var thread = new Thread(() => CancelDownload());
            thread.Start();
        }

        public void CancelDownload()
        {
            IsDownloading = false;

            if (_webClient == null)
            {
                return;
            }

            _webClient.CancelAsync();
        }
    }
}
