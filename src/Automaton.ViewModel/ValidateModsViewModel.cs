using Alphaleonis.Win32.Filesystem;
using Autofac;
using Automaton.Model;
using Automaton.Model.HandyUtils.Interfaces;
using Automaton.Model.Interfaces;
using Automaton.ViewModel.Controllers;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;
using Automaton.ViewModel.Utilities;
using Automaton.ViewModel.Utilities.Interfaces;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Async;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Automaton.ViewModel
{
    public class ValidateModsViewModel : ViewModelBase, IValidateModsViewModel
    {
        private readonly IViewController _viewController;
        private readonly IFileSystemBrowser _fileSystemBrowser;
        private readonly IDialogController _dialogController;
        private readonly ILifetimeData _lifetimeData;
        private readonly INXMRoute _NXMRoute;
        private readonly INexusApi _nexusApi;
        private readonly IDownloadQueue _downloadQueue;
        private readonly ILogger _logger;

        public AsyncCommand ScanDirectoryCommand => new AsyncCommand(ScanDirectory);
        public GenericAsyncCommand<ExtendedArchive> FindAndValidateModCommand => new GenericAsyncCommand<ExtendedArchive>(FindAndValidateMod);
        public GenericAsyncCommand<ExtendedArchive> CancelDownloadCommand => new GenericAsyncCommand<ExtendedArchive>(CancelDownload);
        public RelayCommand<ExtendedArchive> OpenNexusLinkCommand => new RelayCommand<ExtendedArchive>(OpenNexusLink);
        public AsyncCommand ToggleAutomatedDownloadCommand => new AsyncCommand(ToggleAutomatedDownload);

        public ObservableCollection<ExtendedArchive> Archives { get; set; }
        public ICollectionView ArchivesView { get; set; }

        public int MissingArchivesCount { get; set; }
        public bool AutodownloadsEnabled { get; set; }

        public ValidateModsViewModel(IComponentContext components)
        {
            _viewController = components.Resolve<IViewController>();
            _fileSystemBrowser = components.Resolve<IFileSystemBrowser>();
            _dialogController = components.Resolve<IDialogController>();
            _lifetimeData = components.Resolve<ILifetimeData>();
            _NXMRoute = components.Resolve<INXMRoute>();
            _nexusApi = components.Resolve<INexusApi>();
            _downloadQueue = components.Resolve<IDownloadQueue>();

            _viewController.ViewIndexChangedEvent += ViewControllerOnViewIndexChangedEvent;
            _NXMRoute.RecieveMessageEvent += _NXMRoute_RecieveMessageEvent;
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

            await Archives.ToList().ParallelForEachAsync(async archive =>
            {
                await archive.SearchInDirAsync(_lifetimeData.DownloadPath);
            }, maxDegreeOfParalellism: 4);

            var controllerThread = new Thread(() =>
            {
                ValidateController();
            });
            controllerThread.Start();

            _dialogController.CloseCurrentDialog();
            _NXMRoute.StartServer();
        }

        private void _NXMRoute_RecieveMessageEvent(string message)
        {
            var pipedData = _NXMRoute.ToPipedData(message);
            var matchingArchives = Archives.Where(x => x.ModId == pipedData.ModId && x.FileId == pipedData.FileId && !x.IsValidationComplete).ToList();

            if (matchingArchives != null)
            {
                foreach (var archive in matchingArchives)
                {
                    archive.AuthenticationParams = pipedData.AuthenticationParams;
                    archive.DownloadThreaded();
                }
            }
        }

        private async Task ScanDirectory()
        {
            var directoryPath = await _fileSystemBrowser.OpenDirectoryBrowserAsync("Select a folder to scan.");

            if (!string.IsNullOrEmpty(directoryPath))
            {
                _dialogController.OpenLoadingDialog();

                await Archives.ToList().ParallelForEachAsync(async archive =>
                {
                    await archive.SearchInDirAsync(directoryPath);
                }, maxDegreeOfParalellism: 4);
            }

            _dialogController.CloseCurrentDialog();
        }

        private void OpenNexusLink(ExtendedArchive archive)
        {
            var test = Archives.ToList().Where(x => x.ArchiveName.ToLower().Contains("draugr")).ToList();

            if (archive.Repository == "Nexus" && !string.IsNullOrEmpty(archive.ModId))
            {
                Process.Start($"https://nexusmods.com/{archive.GameName}/mods/{archive.ModId}/");
            }

            else
            {
                Process.Start($"https://www.google.com/search?q={Path.GetFileNameWithoutExtension(archive.ArchiveName)}");
            }
        }

        private async Task FindAndValidateMod(ExtendedArchive archive)
        {
            var filePath = await _fileSystemBrowser.OpenFileBrowserAsync($"{archive.Name}|{archive.ArchiveName}|All Matching Extensions|*{Path.GetExtension(archive.ArchiveName)}|All Files|*.*", 
                $"Please select the matching mod archive: {archive.ArchiveName}");

            if (!string.IsNullOrEmpty(filePath))
            {
                _dialogController.OpenLoadingDialog();

                await archive.TryLoadAsync(filePath);
            }

            _dialogController.CloseCurrentDialog();
        }

        private async Task CancelDownload(ExtendedArchive archive)
        {
            archive.CancelDownload();
        }

        private async Task ToggleAutomatedDownload()
        {
            AutodownloadsEnabled = !AutodownloadsEnabled;

            if (AutodownloadsEnabled)
            {
                if (_nexusApi.IsPremium && _nexusApi.IsLoggedIn)
                {
                    foreach (var thing in Archives)
                    {
                        _downloadQueue.Enqueue(thing);
                    }
                }

                else
                {
                    _dialogController.OpenLogDialog("You must be a Nexus Premium member to use the auto-downloading feature of Automaton.");
                    AutodownloadsEnabled = false;
                }
            }

            if (!AutodownloadsEnabled)
            {
                _downloadQueue.ClearQueue();

                foreach (var archive in Archives)
                {
                    archive.CancelDownloadThreaded();
                }
            }
        }

        private void ValidateController()
        {
            var startingMissingModsCount = Archives.ToList().Count(x => !x.IsValidationComplete);
            var lastAutodownloadsStatus = AutodownloadsEnabled;

            while (true)
            {
                MissingArchivesCount = Archives.ToList().Count(x => !x.IsValidationComplete);

                if (MissingArchivesCount != startingMissingModsCount)
                {
                    startingMissingModsCount = MissingArchivesCount;

                    Application.Current.Dispatcher.BeginInvoke(new Action(() => ArchivesView.Refresh()));
                }

                if (MissingArchivesCount == 0)
                {
                    _viewController.IncrementCurrentViewIndex();

                    return;
                }

                if (!AutodownloadsEnabled && lastAutodownloadsStatus)
                {
                    lastAutodownloadsStatus = AutodownloadsEnabled;

                    foreach (var archive in Archives)
                    {
                        archive.CancelDownload();
                    }
                }

                Thread.Sleep(500);
            }
        }

        private bool ArchivesViewFilter(object item)
        {
            var archive = item as ExtendedArchive;

            return !archive.IsValidationComplete;
        }
    }
}
