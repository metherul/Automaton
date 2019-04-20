using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private readonly IFileSystemBrowser _fileSystemBrowser;
        private readonly INxmHandle _nxmHandle;
        private readonly IApiBase _apiBase;
        private readonly IInstallBase _installBase;
        private readonly IDialogController _dialogController;
        private readonly ILogger _logger;

        public RelayCommand ScanDirectoryCommand => new RelayCommand(ScanDirectory);
        public RelayCommand<ExtendedMod> OpenModSourceUrlCommand => new RelayCommand<ExtendedMod>(OpenNexusLink);
        public RelayCommand<ExtendedMod> FindAndValidateModCommand => new RelayCommand<ExtendedMod>(FindAndValidateMod);
        public RelayCommand<ExtendedMod> OpenNexusLinkCommand => new RelayCommand<ExtendedMod>(OpenNexusLink);

        public ObservableCollection<ExtendedMod> ModsList { get; set; } = new ObservableCollection<ExtendedMod>();

        public int RemainingMissingModCount { get; set; }

        public bool IsInitialValidating { get; set; } = true;
        public bool AutodownloadsEnabled { get; set; }
        public bool IsUserPremium { get; set; }
        
        public ValidateModsViewModel(IComponentContext components)
        {
            _viewController = components.Resolve<IViewController>();
            _fileSystemBrowser = components.Resolve<IFileSystemBrowser>();
            _nxmHandle = components.Resolve<INxmHandle>();
            _apiBase = components.Resolve<IApiBase>();
            _installBase = components.Resolve<IInstallBase>();
            _dialogController = components.Resolve<IDialogController>();
            _logger = components.Resolve<ILogger>();

            _viewController.ViewIndexChangedEvent += ViewControllerOnViewIndexChangedEvent;
            _nxmHandle.RecievedPipedDataEvent += NxmHandle_RecievedPipedDataEvent;
        }

        private void ViewControllerOnViewIndexChangedEvent(object sender, int e)
        {
            if (e != (int)ViewIndex.ValidateMods)
            {
                return;
            }

            ModsList = new ObservableCollection<ExtendedMod>(_installBase.ModpackMods);

            ValidateMods();
            _nxmHandle.StartServer();

            IsUserPremium = _apiBase.IsUserLoggedIn() && _apiBase.IsUserPremium();

            IsInitialValidating = false;
        }

        private void NxmHandle_RecievedPipedDataEvent(object sender, PipedData e)
        {
            
        }


        private void InitializeAutoDownloader()
        {
        }

        private async void ValidateMods(string directoryPath = "")
        {
            var directoryTarget = _installBase.DownloadsDirectory;

            if (!string.IsNullOrEmpty(directoryPath))
            {
                directoryTarget = directoryPath;
            }

            var directoryContents = Directory.GetFiles(directoryTarget, "*.*", SearchOption.TopDirectoryOnly).ToList();

            foreach (var mod in ModsList.Where(x => !x.IsValidationComplete))
            {
                await mod.FindValidDirectoryArchiveAsync(directoryContents);
            }
        }


        private void ScanDirectory()
        {
            var directoryPath = _fileSystemBrowser.OpenDirectoryBrowserAsync("Select a folder to scan for mod archives.").Result;

            if (!string.IsNullOrEmpty(directoryPath))
            {
                ValidateMods(directoryPath);
            }
        }

        private void FindAndValidateMod(ExtendedMod mod)
        {
            
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
