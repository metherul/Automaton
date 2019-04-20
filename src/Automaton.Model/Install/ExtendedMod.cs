using Automaton.Model.Modpack.Base;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Automaton.Model.Install
{
    public class ExtendedMod : Mod, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string DisplayName { get; set; }
        public string FilePath { get; set; }

        public int CurrentDownloadProgress { get; set; }

        public bool IsIndeterminateProcess { get; set; }
        public bool IsModOrganizer { get; set; }
        public bool IsDownloading { get; set; }

        public void Initialize()
        {

        }

        public void InstallMod()
        {

        }

        public async Task InstallModAsync()
        {
            
        }

        public void DownloadMod()
        {

        }

        public async Task DownloadModAsync()
        {

        }

        public void ValidateArchive(string archivePath)
        {

        }

        public async Task ValidateArchiveAsync(string archivePath)
        {

        }

        public void FindValidDirectoryArchive(string directoryPath)
        {

        }

        public async Task FindValidDirectoryArchiveAsync(string directoryPath)
        {

        }
    }
}
