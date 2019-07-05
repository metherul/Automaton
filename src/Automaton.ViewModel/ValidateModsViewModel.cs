using Alphaleonis.Win32.Filesystem;
using Autofac;
using Automaton.Model;
using Automaton.Model.Interfaces;
using Automaton.ViewModel.Controllers;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;
using Automaton.ViewModel.Utilities;
using Automaton.ViewModel.Utilities.Interfaces;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Automaton.ViewModel
{
    public class ValidateModsViewModel : ViewModelBase, IValidateModsViewModel
    {
        private readonly IViewController _viewController;
        private readonly IFileSystemBrowser _fileSystemBrowser;
        private readonly IDialogController _dialogController;
        private readonly ILifetimeData _lifetimeData;
        private readonly ILogger _logger;

        public AsyncCommand ScanDirectoryCommand => new AsyncCommand(ScanDirectory);
        public RelayCommand<ExtendedArchive> OpenNexusLinkCommand => new RelayCommand<ExtendedArchive>(OpenNexusLink);

        public ObservableCollection<ExtendedArchive> Archives { get; set; }
        public ICollectionView ArchivesView { get; set; }
        public ValidateModsViewModel(IComponentContext components)
        {
            _viewController = components.Resolve<IViewController>();
            _fileSystemBrowser = components.Resolve<IFileSystemBrowser>();
            _dialogController = components.Resolve<IDialogController>();
            _lifetimeData = components.Resolve<ILifetimeData>();

            _viewController.ViewIndexChangedEvent += ViewControllerOnViewIndexChangedEvent;
        }

        private async void ViewControllerOnViewIndexChangedEvent(object sender, int e)
        {
            if (e != (int)ViewIndex.ValidateMods)
            {
                return;
            }

            _dialogController.OpenLoadingDialog();

            Archives = new ObservableCollection<ExtendedArchive>(_lifetimeData.Archives);

            ArchivesView = CollectionViewSource.GetDefaultView(Archives);
            ArchivesView.Filter = ArchivesViewFilter;

            foreach (var archive in Archives)
            {
                await archive.SearchInDirAsync(_lifetimeData.DownloadPath);
            }

            ArchivesView.Refresh();

            _dialogController.CloseCurrentDialog();
        }

        private async Task ScanDirectory()
        {
            var directoryPath = await _fileSystemBrowser.OpenDirectoryBrowserAsync("Select a folder to scan.");

            if (!string.IsNullOrEmpty(directoryPath))
            {
                _dialogController.OpenLoadingDialog();

                foreach (var archive in Archives)
                {
                    await archive.SearchInDirAsync(directoryPath);
                }

                _dialogController.CloseCurrentDialog();
            }

        }

        private void OpenNexusLink(ExtendedArchive archive)
        {
            if (archive.Repository == "Nexus" && !string.IsNullOrEmpty(archive.ModId))
            {
                Process.Start($"https://nexusmods.com/{archive.GameName}/mods/{archive.ModId}/");
            }

            else
            {
                Process.Start($"https://www.google.com/search?q={Path.GetFileNameWithoutExtension(archive.ArchiveName)}");
            }
        }

        private bool ArchivesViewFilter(object item)
        {
            var archive = item as ExtendedArchive;

            return !File.Exists(archive.ArchivePath);
        }
    }
}
