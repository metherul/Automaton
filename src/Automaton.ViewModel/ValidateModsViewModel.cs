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
using Autofac;
using Automaton.Model.Install;
using Automaton.Model.Install.Intefaces;
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

        private bool _missingModsLocked;

        public RelayCommand ScanDirectoryCommand => new RelayCommand(ScanDirectory);
        public RelayCommand FindAndValidateModFileCommand { get; set; }
        public RelayCommand OpenModSourceUrlCommand { get; set; }
        public RelayCommand<ExtendedMod> FindAndValidateModCommand => new RelayCommand<ExtendedMod>(FindAndValidateMod);
        public RelayCommand<ExtendedMod> OpenNexusLinkCommand => new RelayCommand<ExtendedMod>(OpenNexusLink);

        public RangeObservableCollection<ExtendedMod> MissingMods { get; set; } = new RangeObservableCollection<ExtendedMod>();

        public int RemainingMissingModCount { get; set; }

        public bool IsInitialValidating { get; set; }
        
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

            _viewController.ViewIndexChangedEvent += ViewControllerOnViewIndexChangedEvent;
            _nxmHandle.RecievedPipedDataEvent += QueueDownload;
            _downloadClient.DownloadUpdate += DownloadUpdate;
        }

        private void ViewControllerController() 
        {
            // Phin would be proud. This will be replaced when I have more time.
            // That never means anything though. This will be around for a while.

            while (true)
            {
                if (MissingMods.Count() == 0 && _missingModsLocked == false)
                {
                    _viewController.IncrementCurrentViewIndex();

                    return;
                }

                Thread.Sleep(10);
            }
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
        }

        private async void QueueDownload(object caller, PipedData pipedData)
        {
            if (!MissingMods.Any(x => x.FileId == pipedData.FileId && x.ModId == pipedData.ModId) || !_apiBase.IsUserLoggedIn())
            {
                return;
            }

            var downloadUrl = await _apiEndpoints.GenerateModDownloadLinkAsync(pipedData);
            var matchingModObject = MissingMods.First(x => x.FileId == pipedData.FileId && x.ModId == pipedData.ModId);

            MissingMods.First(x => x == matchingModObject).IsIndeterminateProcess = true;

            if (matchingModObject == null)
            {
                return;
            }

            _downloadClient.QueueDownload(downloadUrl, matchingModObject);
        }

        private void DownloadUpdate(object sender, ExtendedMod e)
        {
            if (e.CurrentDownloadProgress == 100)
            {
                _installBase.ModpackMods.Where(x => x.Md5 == e.Md5).ToList()
                    .ForEach(x => x.FilePath = e.FilePath);

                var matchingMods = MissingMods.Where(x => x.Md5 == e.Md5).ToList();

                foreach (var matchingMod in matchingMods)
                {
                    Application.Current.Dispatcher.BeginInvoke((Action)delegate
                    {
                        _missingModsLocked = true;

                        if (MissingMods.IndexOf(matchingMod) != -1)
                        {
                            MissingMods.RemoveAt(MissingMods.IndexOf(matchingMod));
                            RemainingMissingModCount--;
                        }

                        _missingModsLocked = false;
                    });
                }
            }

            else
            {
                var missingMods = MissingMods.ToList();
                foreach (var matchingMissingMod in missingMods.Where(x => x.Md5 == e.Md5).ToList())
                {
                    Application.Current.Dispatcher.BeginInvoke((Action)delegate
                    {
                        _missingModsLocked = true;

                        var index = MissingMods.IndexOf(matchingMissingMod);
                        if (index == -1)
                        {
                            return;
                        }

                        MissingMods[MissingMods.IndexOf(matchingMissingMod)].CurrentDownloadProgress = e.CurrentDownloadProgress;
                        _missingModsLocked = false;
                    });
                }
            }
        }

        private async Task ValidateMods()
        {
            var missingMods = await _validate.GetMissingModsAsync(new List<string>());

            await Application.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                MissingMods.AddRange(missingMods);
            });

            RemainingMissingModCount = _installBase.ModpackMods.Count;

            Task.Factory.StartNew(ViewControllerController);
        }

        private async void ScanDirectory()
        {
            var directoryToScan = await _fileSystemBrowser.OpenDirectoryBrowserAsync("Select a folder to scan for mods.");

            if (string.IsNullOrEmpty(directoryToScan))
            {
                return;
            }

            var missingMods = await _validate.FilterMissingModsAsync(directoryToScan);

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
            var possibleArchiveMatch = await _fileSystemBrowser.OpenFileBrowserAsync($"{mod.ModName}|{mod.FileName}|All Matching Extensions|*{Path.GetExtension(mod.FileName)}|All Files|*.*",
                $"Please select the matching mod archive: {mod.FileName}");

            if (string.IsNullOrEmpty(possibleArchiveMatch))
            {
                return;
            }

            var filteredMissingMods = _validate.ValidateTargetModArchive(possibleArchiveMatch, MissingMods.ToList());

            RemainingMissingModCount = filteredMissingMods.Count;

            await Application.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                _missingModsLocked = true;
                MissingMods = new RangeObservableCollection<ExtendedMod>();
                MissingMods.AddRange(filteredMissingMods);
                _missingModsLocked = false;
            });
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
