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

namespace Automaton.Model
{
    public class ExtendedArchive : SourceArchive, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Mod _parentMod;

        private IArchiveHandle _archiveHandle;
        private ILifetimeData _lifetimeData;
        private IHandyUtils _handyUtils;

        public string ArchivePath { get; set; }

        public int DownloadPercentage { get; set; }

        public ExtendedArchive Initialize(IComponentContext components, Mod parentMod)
        {
            // Load in required modules
            _parentMod = parentMod;

            _archiveHandle = components.Resolve<IArchiveHandle>();
            _lifetimeData = components.Resolve<ILifetimeData>();
            _handyUtils = components.Resolve<IHandyUtils>();

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


        public void Download()
        {
            // Initialize the webClient and set required headers
            var webClient = new WebClient();

            webClient.DownloadProgressChanged += (sender, e) =>
            {
                DownloadPercentage = e.ProgressPercentage;
            };

            webClient.DownloadFileCompleted += (sender, e) =>
            {
                TryLoad(Path.Combine(_lifetimeData.DownloadPath, ArchiveName));
            };

            webClient.DownloadFile("", Path.Combine(_lifetimeData.DownloadPath, ArchiveName));
        }

        public async Task DownloadAsync()
        {
            await Task.Run(Download);
        }


        public void TryLoad(string archivePath)
        {
            // For performance we can check the file length first
            if (File.GetSize(archivePath) != Size)
            {
                return;
            }

            var test = MD5;

            // We want to try to load in the archive, but we want to check it against the metadata we have stored first
            if (_handyUtils.GetMd5FromFile(archivePath) != MD5)
            {
                return;
            }

            // We're good. This isn't quick, but it's accurate. We can add on further logic here further down the line.
            ArchivePath = archivePath;
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
