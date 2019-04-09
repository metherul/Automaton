using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Autofac;
using Automaton.Model.Install;
using Automaton.Model.Install.Intefaces;
using Automaton.Model.Interfaces;
using Automaton.Model.NexusApi;
using Automaton.Model.NexusApi.Interfaces;
using Automaton.ViewModel.Controllers;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;
using Automaton.ViewModel.Utilities.Interfaces;
using GalaSoft.MvvmLight.Command;

namespace Automaton.ViewModel
{
    public class ValidateModsViewModel : ViewModelBase, IValidateModsViewModel
    {
        private readonly IViewController _viewController;
        private readonly IValidate _validate;
        private readonly IFileSystemBrowser _fileSystemBrowser;
        private readonly INxmHandle _nxmHandle;
        private readonly IApiEndpoints _apiEndpoints;
        private readonly IDownloadClient _downloadClient;
        private readonly IApiBase _apiBase;
        private readonly IInstallBase _installBase;
        private readonly IDialogController _dialogController;
        private readonly ILogger _logger;

        private List<ExtendedMod> _failedDownloads = new List<ExtendedMod>();

        private Stopwatch _queueTimer;

        private int _readerIndex;

        private bool _missingModsLocked;

        public RelayCommand ScanDirectoryCommand => new RelayCommand(ScanDirectory);
        public RelayCommand FindAndValidateModFileCommand { get; set; }
        public RelayCommand OpenModSourceUrlCommand { get; set; }
        public RelayCommand<ExtendedMod> FindAndValidateModCommand => new RelayCommand<ExtendedMod>(FindAndValidateMod);
        public RelayCommand<ExtendedMod> OpenNexusLinkCommand => new RelayCommand<ExtendedMod>(OpenNexusLink);

        public RangeObservableCollection<ExtendedMod> MissingMods { get; set; } = new RangeObservableCollection<ExtendedMod>();

        public int RemainingMissingModCount { get; set; }

        public bool IsInitialValidating { get; set; }
        public bool QueueDownloads { get; set; }
        public bool AutodownloadsEnabled { get; set; }
        public bool IsUserPremium { get; set; }
        
        public ValidateModsViewModel(IComponentContext components)
        {
            _viewController = components.Resolve<IViewController>();
            _validate = components.Resolve<IValidate>();
            _fileSystemBrowser = components.Resolve<IFileSystemBrowser>();
            _nxmHandle = components.Resolve<INxmHandle>();
            _apiEndpoints = components.Resolve<IApiEndpoints>();
            _downloadClient = components.Resolve<IDownloadClient>();
            _apiBase = components.Resolve<IApiBase>();
            _installBase = components.Resolve<IInstallBase>();
            _dialogController = components.Resolve<IDialogController>();
            _logger = components.Resolve<ILogger>();

            _viewController.ViewIndexChangedEvent += ViewControllerOnViewIndexChangedEvent;
            _nxmHandle.RecievedPipedDataEvent += QueueDownload;
            _downloadClient.DownloadUpdate += DownloadUpdate;

            BindingOperations.EnableCollectionSynchronization(MissingMods, _missingModsLocked);
        }

        private async void ViewControllerOnViewIndexChangedEvent(object sender, int e)
        {
            if (e != (int)ViewIndex.ValidateMods)
            {
                return;
            }

            IsInitialValidating = true;

            await ValidateMods();
            _nxmHandle.StartServer();

            IsInitialValidating = false;

            IsUserPremium = _apiBase.IsUserLoggedIn() && _apiBase.IsUserPremium();
        }

        private void ValidateModsController() 
        {
            // Phin would be proud. This will be replaced when I have more time.
            // That never means anything though. This will be around for a while.

            var lastAutodownloadsStatus = AutodownloadsEnabled;

            while (true)
            {
                if (MissingMods.Count() == 0 && _missingModsLocked == false)
                {
                    _viewController.IncrementCurrentViewIndex();

                    return;
                }

                if (AutodownloadsEnabled && !lastAutodownloadsStatus)
                {
                    lastAutodownloadsStatus = AutodownloadsEnabled;

                    if (_apiBase.IsUserLoggedIn() && _apiBase.IsUserPremium())
                    {
                        InitializeAutoDownloader();
                    }

                    else
                    {
                        _dialogController.OpenLogDialog("You must be a Nexus Premium member to use the auto-downloading feature of Automaton.");
                        AutodownloadsEnabled = false;
                    }
                }

                if (!AutodownloadsEnabled && lastAutodownloadsStatus)
                {
                    lastAutodownloadsStatus = AutodownloadsEnabled;
                    _downloadClient.PurgeQueue();
                }

                Thread.Sleep(100);
            }
        }

        private void InitializeAutoDownloader()
        {
            if (_apiBase.IsUserPremium() && _apiBase.IsUserLoggedIn())
            {
                var missingMods = MissingMods.ToList();

                foreach (var mod in missingMods)
                {
                    if (!_failedDownloads.Contains(mod))
                    {
                        QueueDownload(mod);
                    }
                }
            }
        }

        private async void QueueDownload(object caller, PipedData pipedData)
        {
            lock (MissingMods)
            {
                if (!MissingMods.ToList().Any(x => x.FileId == pipedData.FileId && x.ModId == pipedData.ModId) || !_apiBase.IsUserLoggedIn())
                {
                    return;
                }

                var downloadUrl = _apiEndpoints.GenerateModDownloadLinkAsync(pipedData).Result;
                var matchingModObject = MissingMods.ToList().First(x => x.FileId == pipedData.FileId && x.ModId == pipedData.ModId);

                MissingMods.ToList().First(x => x == matchingModObject).IsIndeterminateProcess = true;

                if (matchingModObject == null)
                {
                    return;
                }

                _downloadClient.QueueDownload(matchingModObject, downloadUrl);
            }
        }

        private void QueueDownload(ExtendedMod mod)
        {
            lock (MissingMods)
            {
                MissingMods.ToList().Where(x => x.Md5 == mod.Md5 && x.FileName == mod.FileName).ToList()
                    .ForEach(x => x.IsIndeterminateProcess = true);
            }

            _downloadClient.QueueDownload(mod, string.Empty);
        }

        private void DownloadUpdate(object sender, (ExtendedMod, bool) e)
        {
            var mod = e.Item1;
            var hasFailed = e.Item2;

            lock (MissingMods)
            {
                if (hasFailed)
                {
                    var invalidMods = MissingMods.ToList().Where(x => x == mod).ToList();

                    foreach (var invalidMod in invalidMods)
                    {
                        var itemIndex = MissingMods.IndexOf(invalidMod);

                        MissingMods.Move(itemIndex, MissingMods.Count - 1);

                        _failedDownloads.Add(mod);
                    }
                }

                if (mod.CurrentDownloadProgress == 100)
                {
                    _installBase.ModpackMods.Where(x => x.Md5 == mod.Md5).ToList()
                        .ForEach(x => x.FilePath = mod.FilePath);

                    _installBase.ModpackMods.Where(x => x.Md5 == mod.Md5).ToList()
                        .ForEach(x => x.IsDownloading = false);

                    var matchingMods = MissingMods.ToList().Where(x => x.Md5 == mod.Md5).ToList(); // Error happens here. Maybe a multithreaded issue?

                    foreach (var matchingMod in matchingMods)
                    {
                        if (MissingMods.ToList().IndexOf(matchingMod) != -1)
                        {
                            MissingMods.RemoveAt(MissingMods.IndexOf(matchingMod));
                            RemainingMissingModCount--;
                        }
                    }
                }

                else
                {
                    MissingMods.Where(x => x.Md5 == mod.Md5).ToList().ForEach(x => x.CurrentDownloadProgress = mod.CurrentDownloadProgress);
                }
            }
        }

        private async Task ValidateMods()
        {
            var missingMods = await _validate.GetMissingModsAsync(new List<string>());

            MissingMods.AddRange(missingMods);

            RemainingMissingModCount = MissingMods.Count;

            Task.Factory.StartNew(ValidateModsController);
        }

        private async void ScanDirectory()
        {
            _downloadClient.PurgeQueue();
            AutodownloadsEnabled = false;

            var directoryToScan = await _fileSystemBrowser.OpenDirectoryBrowserAsync("Select a folder to scan for mods.");

            if (string.IsNullOrEmpty(directoryToScan))
            {
                return;
            }

            var missingMods = await _validate.FilterMissingModsAsync(directoryToScan);
            var removedMods = MissingMods.Where(x => !missingMods.Contains(x));

            if (removedMods.Any())
            {
                foreach (var mod in removedMods)
                {
                    _downloadClient.PurgeQueue(mod);
                }
            }

            await Application.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                _missingModsLocked = true;
                MissingMods = new RangeObservableCollection<ExtendedMod>();
                MissingMods.AddRange(missingMods);
                _missingModsLocked = false;
            });

            RemainingMissingModCount =  MissingMods.Count;
        }

        private async void FindAndValidateMod(ExtendedMod mod)
        {
            // Remove the mod from the queue
            _downloadClient.PurgeQueue(mod);

            var possibleArchiveMatch = await _fileSystemBrowser.OpenFileBrowserAsync($"{mod.ModName}|{mod.FileName}|All Matching Extensions|*{Path.GetExtension(mod.FileName)}|All Files|*.*",
                $"Please select the matching mod archive: {mod.FileName}");

            if (string.IsNullOrEmpty(possibleArchiveMatch))
            {
                lock (MissingMods)
                {
                    MissingMods.Move(MissingMods.IndexOf(mod), MissingMods.Count - 1);
                }
                QueueDownload(mod);

                return;
            }

            var filteredMissingMods = _validate.ValidateTargetModArchive(possibleArchiveMatch, MissingMods.ToList());

            RemainingMissingModCount = filteredMissingMods.Count;

            lock (MissingMods)
            {
                MissingMods = new RangeObservableCollection<ExtendedMod>();
                MissingMods.AddRange(filteredMissingMods);
            }
        }

        private void OpenNexusLink(ExtendedMod mod)
        {
            if (mod.Repository == "NexusMods")
            {
                Process.Start($"https://nexusmods.com/{mod.TargetGame.ToLower()}/mods/{mod.ModId}?tab=files");
            }

            else
            {
                Process.Start($"https://www.google.com/search?q={Path.GetFileNameWithoutExtension(mod.FileName)}");
            }
        }
    }

    public class RangeObservableCollection<T> : ObservableCollection<T>
    {
        private bool _suppressNotification;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressNotification)
                base.OnCollectionChanged(e);
        }

        public void AddRange(IEnumerable<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            _suppressNotification = true;

            foreach (var item in list)
            {
                Add(item);
            }

            _suppressNotification = false;

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
