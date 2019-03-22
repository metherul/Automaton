using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
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

        public RelayCommand ScanDirectoryCommand => new RelayCommand(ScanDirectory);
        public RelayCommand FindAndValidateModFileCommand { get; set; }
        public RelayCommand OpenModSourceUrlCommand { get; set; }
        public RelayCommand<ExtendedMod> FindAndValidateModCommand => new RelayCommand<ExtendedMod>(FindAndValidateMod);
        public RelayCommand<ExtendedMod> OpenNexusLinkCommand => new RelayCommand<ExtendedMod>(OpenNexusLink);

        public RangeObservableCollection<ExtendedMod> MissingMods { get; set; } = new RangeObservableCollection<ExtendedMod>();

        public int StartingMissingModCount { get; set; }
        public int ValidatedModCount { get; set; }

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
            _nxmHandle.RecievedPipedDataEvent += HandlePipedData;
        }

        private void ViewControllerOnViewIndexChangedEvent(object sender, int e)
        {
            if (e != (int)ViewIndex.ValidateMods)
            {
                return;
            }

            ValidateMods();
            _nxmHandle.StartServer();
        }

        private async void HandlePipedData(object caller, PipedData pipedData)
        {
            if (!MissingMods.Any(x => x.FileId == pipedData.FileId && x.ModId == pipedData.ModId) || !_apiBase.IsUserLoggedIn())
            {
                return;
            }

            var downloadUrl = await _apiEndpoints.GenerateModDownloadLinkAsync(pipedData);
            var missingModObject = MissingMods.First(x => x.FileId == pipedData.FileId && x.ModId == pipedData.ModId);

            if (!MissingMods.Any())
            {
                return;
            }

            await _downloadClient.DownloadFileAsync(downloadUrl, missingModObject);
        }

        private async void ValidateMods()
        {
            var missingMods = await _validate.GetMissingModsAsync(new List<string>());

            await Application.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                MissingMods.AddRange(missingMods);
            });

            StartingMissingModCount = _installBase.ModpackMods.Count;
            ValidatedModCount = StartingMissingModCount - missingMods.Count;
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
                MissingMods = new RangeObservableCollection<ExtendedMod>();
                MissingMods.AddRange(missingMods);
            });

            ValidatedModCount = StartingMissingModCount - MissingMods.Count;
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

            ValidatedModCount = StartingMissingModCount - filteredMissingMods.Count;

            await Application.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                MissingMods = new RangeObservableCollection<ExtendedMod>();
                MissingMods.AddRange(filteredMissingMods);
            });
        }

        private async void OpenNexusLink(ExtendedMod mod)
        {
            Process.Start($"https://nexusmods.com/{mod.TargetGame.ToLower()}/mods/{mod.ModId}?tab=files");
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
