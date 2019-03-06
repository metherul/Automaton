using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

        public RelayCommand ScanDirectoryCommand => new RelayCommand(ScanDirectory);
        public RelayCommand FindAndValidateModFileCommand { get; set; }
        public RelayCommand OpenModSourceUrlCommand { get; set; }

        public RangeObservableCollection<ExtendedMod> MissingMods { get; set; } = new RangeObservableCollection<ExtendedMod>();

        public ValidateModsViewModel(IComponentContext components)
        {
            _viewController = components.Resolve<IViewController>();
            _validate = components.Resolve<IValidate>();
            _fileSystemBrowser = components.Resolve<IFileSystemBrowser>();
            _nxmHandle = components.Resolve<INxmHandle>();

            _viewController.ViewIndexChangedEvent += ViewControllerOnViewIndexChangedEvent;
            _nxmHandle.RecievedPipedDataEvent += HandlePipedData;

            _nxmHandle.ConnectClient();
        }

        private void ViewControllerOnViewIndexChangedEvent(object sender, int e)
        {
            if (e != (int)ViewIndex.ValidateMods)
            {
                return;
            }

            ValidateMods();
        }

        private void HandlePipedData(object caller, PipedData pipedData)
        {

        }

        private async void ValidateMods()
        {
            var missingMods = await _validate.GetMissingModsAsync(new List<string>());
            MissingMods.AddRange(missingMods);
        }

        private async void ScanDirectory()
        {
            var directoryToScan = await _fileSystemBrowser.OpenDirectoryBrowserAsync("Select a folder to scan for mods.");
            var missingMods = await _validate.FilterMissingModsAsync(directoryToScan);

            MissingMods = new RangeObservableCollection<ExtendedMod>();
            MissingMods.AddRange(missingMods);
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
