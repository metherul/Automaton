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

        private WebClient _webClient;

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
            var installationDirectory = Path.Combine(_lifetimeData.InstallPath, "mods", _parentMod.Name);
            //var filePairings = _parentMod.InstallPlans.SelectMany(x => x.FilePairings);

            var plan = _parentMod.InstallPlans.Where(p => p.SourceArchive.SHA256 == SHA256).First();

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

            // Get a dictionary of all the files we need to copy indexed by their name in the archive
            var extract_files = plan.FilePairings.GroupBy(p => p.From).ToDictionary(p => p.Key);


            // Let's pre-create all the directories in the mod folder so we don't have to check
            // for missing folders during the install.
            var directories = (from entry in extract_files
                               from to in entry.Value
                               let full_path = Path.Combine(installationDirectory, to.To)
                               select Path.GetDirectoryName(full_path)).Distinct();

            foreach (var dir in directories)
                Directory.CreateDirectory(dir);


            using (var file = new ArchiveFile(ArchivePath))
            {
                file.Extract(entry => {
                    if (extract_files.ContainsKey(entry.FileName))
                    {
                        // We may need to copy the same file to multiple locations so extract to the first one, 
                        // we'll copy this file around later.
                        var to = extract_files[entry.FileName].First();
                        var path = Path.Combine(installationDirectory, to.To);
                        return File.OpenWrite(Path.Combine(installationDirectory, path));
                    }

                    return null;
                });
            }

            // Now that we've installed all the files, copy around any files that exist in more than one location
            // in the mod.
            foreach (var copy_group in extract_files.Select(e => e.Value).Where(es => es.Count() > 1))
            {
                var from = copy_group.First();
                foreach (var to in copy_group.Skip(1))
                {
                    File.Copy(Path.Combine(installationDirectory, from.To),
                              Path.Combine(installationDirectory, to.To));
                }
            }

            // <INSERT BINARY PATCHING CODE HERE>

            // And we're done!


        }

        public async Task InstallAsync()
        {
            await Task.Run(Install);
        }

        public async Task DownloadAsync()
        {
            IsDownloading = true;
            _lifetimeData.CurrentDownloads++;

            // We need to grab the download URL for this given item
            var downloadLink = await _nexusApi.GetArchiveDownloadUrl(this);

            if (downloadLink == null)
            {
                IsDownloading = false;
                _lifetimeData.CurrentDownloads--;
                _dialogRedirector.RouteLog($"Failed to get Nexus download link for {ArchiveName}. This file must be downloaded manually. If this issue persists, please contact the modpack developer.");

                return;
            }

            // Initialize the webClient and set required headers
            _webClient = new WebClient();
            _webClient.Headers.Add("User-Agent", _lifetimeData.UserAgent);

            var archivePath = Path.Combine(_lifetimeData.DownloadPath, ArchiveName);
            var partPath = Path.Combine(_lifetimeData.DownloadPath, ArchiveName + ".part");

            if (File.Exists(partPath))
            {
                IsValidationComplete = true;
                IsDownloading = false;
                _lifetimeData.CurrentDownloads--;

                ArchivePath = archivePath;

                return; // A hack

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

        public void CancelDownload()
        {
            _webClient.CancelAsync();
        }

        public void TryLoad(string archivePath)
        {
            // For performance we can check the file length first
            if (File.GetSize(archivePath) != Size)
            {
                return;
            }

            // We want to try to load in the archive, but we want to check it against the metadata we have stored first
            if (Utils.FileSHA256(archivePath) != SHA256)
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
