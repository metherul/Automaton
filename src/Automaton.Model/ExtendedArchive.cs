using Autofac;
using Automaton.Model.Interfaces;
using Alphaleonis.Win32.Filesystem;
using System.Linq;
using SevenZipExtractor;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Automaton.Common.Model;
using System.ComponentModel;
using System;
using Automaton.Model.HandyUtils;
using Automaton.Common;
using Automaton.Model.HandyUtils.Interfaces;
using System.Threading;
using System.Diagnostics;

namespace Automaton.Model
{
    public class ExtendedArchive : SourceArchive, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Mod _parentMod;

        private IArchiveHandle _archiveHandle;
        private ILifetimeData _lifetimeData;
        private INexusApi _nexusApi;
        private IDialogRedirector _dialogRedirector;

        public string ArchivePath { get; set; }
        public double MbPerSecond { get; set; }
        public int DownloadPercentage { get; set; }
        public bool IsDownloading { get; set; }
        public bool IsValidationComplete { get; set; }

        public ExtendedArchive Initialize(IComponentContext components, Mod parentMod)
        {
            // Load in required modules
            _parentMod = parentMod;

            _archiveHandle = components.Resolve<IArchiveHandle>();
            _lifetimeData = components.Resolve<ILifetimeData>();
            _nexusApi = components.Resolve<INexusApi>();
            _dialogRedirector = components.Resolve<IDialogRedirector>();

            return this;
        }

        public void Install()
        {
            var installationDirectory = Path.Combine(_lifetimeData.InstallPath, _parentMod.Name);
            var filePairings = _parentMod.InstallPlans.SelectMany(x => x.FilePairings);

            // Verify to ensure that the SourceArchive path exists
            if (!File.Exists(ArchivePath))
            {
                return;
            }

            // Verify to ensure that the mod's installation directory exists
            if (!Directory.Exists(installationDirectory))
            {
                Directory.CreateDirectory(installationDirectory);
            }

            // Spool up new ArchiveHandle and filter each entry by their name
            var archiveContents = _archiveHandle.New(ArchivePath).GetContents();
            var installDictionary = new Dictionary<Entry, string>();

            // Build the installation dictionary
            foreach (var filePairing in filePairings)
            {
                installDictionary.Add(archiveContents
                    .First(x => x.FileName == filePairing.From), Path.Combine(installationDirectory, filePairing.To));
            }

            // Extract each into their target location 
        }

        public async Task InstallAsync()
        {
            await Task.Run(Install);
        }

        public async Task DownloadAsync()
        {
            IsDownloading = true;

            // We need to grab the download URL for this given item
            var downloadLink = await _nexusApi.GetArchiveDownloadUrl(this);

            if (downloadLink == null)
            {
                IsDownloading = false;
                _dialogRedirector.RouteLog($"Failed to get Nexus download link for {ArchiveName}. This file must be downloaded manually. If this issue persists, please contact the modpack developer.");

                return;
            }

            // Initialize the webClient and set required headers
            var webClient = new WebClient();

            webClient.Headers.Add("User-Agent", _lifetimeData.UserAgent);

            var archivePath = Path.Combine(_lifetimeData.DownloadPath, ArchiveName);
            var partPath = Path.Combine(_lifetimeData.DownloadPath, ArchiveName + ".part");

            if (File.Exists(partPath))
            {
                IsValidationComplete = true;
                IsDownloading = false;

                ArchivePath = archivePath;

                return; // A hack

                File.Delete(partPath);
            }

            _lifetimeData.CurrentDownloads++;

            var lastBytesRecieved = (long)0;
            var dateTime = DateTime.Now;

            webClient.DownloadProgressChanged += async (sender, e) =>
            {
                var timeSpan = DateTime.Now - dateTime;
                lastBytesRecieved = e.BytesReceived - lastBytesRecieved;

                var bytesPerSecond = lastBytesRecieved / (double)timeSpan.TotalSeconds;

                MbPerSecond = Math.Round((double)bytesPerSecond / (double)1024 / (double)1024, 2);
                DownloadPercentage = (int)(1000 * ((double)e.BytesReceived / (double)e.TotalBytesToReceive));
            };

            webClient.DownloadFileCompleted += async (sender, e) =>
            {
                _lifetimeData.CurrentDownloads--;

                IsDownloading = false;

                if (File.Exists(archivePath))
                {
                    File.Delete(archivePath);
                }

                File.Move(partPath, archivePath);

                await TryLoadAsync(Path.Combine(_lifetimeData.DownloadPath, ArchiveName));

                if (File.Exists(ArchivePath))
                {
                    webClient.Dispose();
                }
            };

            webClient.DownloadFileAsync(new Uri(downloadLink), partPath);
        }

        public void DownloadThreaded()
        {
            var thread = new Thread(() => DownloadAsync());
            thread.Start();
        }

        public void TryLoad(string archivePath)
        {
            // For performance we can check the file length first
            if (File.GetSize(archivePath) != Size)
            {
                return;
            }

            // We want to try to load in the archive, but we want to check it against the metadata we have stored first
            if (Utils.FileMD5(archivePath) != MD5)
            {
                return;
            }

            // We're good. This isn't quick, but it's accurate. We can add on further logic here further down the line.
            ArchivePath = archivePath;

            IsValidationComplete = true;
        }

        public async Task TryLoadAsync(string archivePath)
        {
            await Task.Run(() => TryLoad(archivePath));
        }

        public void SearchInDir(string directoryPath)
        {
            var dirContents = new Queue<string>(Directory.GetFiles(directoryPath, "*.*", System.IO.SearchOption.TopDirectoryOnly));

            while (!File.Exists(ArchivePath) && dirContents.Any())
            {
                var file = dirContents.Dequeue();

                TryLoad(file);
            }
        }

        public async Task SearchInDirAsync(string directoryPath)
        {
            await Task.Run(() => SearchInDir(directoryPath));
        }
    }
}
