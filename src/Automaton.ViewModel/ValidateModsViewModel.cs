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
        private readonly ILogger _logger;

        public AsyncCommand ScanDirectoryCommand => new AsyncCommand(ScanDirectory);
        public GenericAsyncCommand<ExtendedArchive> FindAndValidateModCommand => new GenericAsyncCommand<ExtendedArchive>(FindAndValidateMod);
        public GenericAsyncCommand<ExtendedArchive> CancelDownloadCommand => new GenericAsyncCommand<ExtendedArchive>(CancelDownload);
        public RelayCommand<ExtendedArchive> OpenNexusLinkCommand => new RelayCommand<ExtendedArchive>(OpenNexusLink);

        public ObservableCollection<ExtendedArchive> Archives { get; set; }
        public ICollectionView ArchivesView { get; set; }

        public int MissingArchivesCount { get; set; }

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

            var controllerThread = new Thread(() =>
            {
                var startingMissingModsCount = Archives.ToList().Count(x => !x.IsValidationComplete);

                while (true)
                {
                    MissingArchivesCount = Archives.ToList().Count(x => !x.IsValidationComplete);

                    if (MissingArchivesCount != startingMissingModsCount)
                    {
                        startingMissingModsCount = MissingArchivesCount;

                        Application.Current.Dispatcher.BeginInvoke(new Action(() => ArchivesView.Refresh()));
                    }
                    // TODO: have metherul wire this up proper
                    if (MissingArchivesCount == 0)
                    {
                        foreach (var archive in _lifetimeData.Archives) {
                            archive.Install();
                        }
                        break;
                    }

                    Thread.Sleep(500);
                }
            });
            controllerThread.Start();

            foreach (var archive in Archives)
            {
                await archive.SearchInDirAsync(_lifetimeData.DownloadPath);
            }

            _dialogController.CloseCurrentDialog();
        }

        private async Task ScanDirectory()
        {
            foreach (var thing in Archives)
            {
                while (_lifetimeData.CurrentDownloads >= 5)
                {
                    await Task.Delay(500);
                }

                thing.DownloadThreaded();

                await Task.Delay(250);
            }

            return;

            var directoryPath = await _fileSystemBrowser.OpenDirectoryBrowserAsync("Select a folder to scan.");

            if (!string.IsNullOrEmpty(directoryPath))
            {
                _dialogController.OpenLoadingDialog();

                await Archives.ToList().ParallelForEachAsync(async archive =>
                {
                    await archive.SearchInDirAsync(directoryPath);
                }, maxDegreeOfParalellism:4);

                //foreach (var archive in Archives)
                //{
                //    await archive.SearchInDirAsync(directoryPath);
                //}
            }

            _dialogController.CloseCurrentDialog();
        }

        private void OpenNexusLink(ExtendedArchive archive)
        {
            archive.DownloadAsync();

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

        private bool ArchivesViewFilter(object item)
        {
            var archive = item as ExtendedArchive;

            return !archive.IsValidationComplete;
        }
    }
}
