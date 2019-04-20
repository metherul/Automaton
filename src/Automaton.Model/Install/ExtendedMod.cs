using Autofac;
using Automaton.Model.Install.Intefaces;
using Automaton.Model.Install.Interfaces;
using Automaton.Model.Modpack.Base;
using Automaton.Model.NexusApi;
using Automaton.Model.NexusApi.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Automaton.Model.Install
{
    public class ExtendedMod : Mod, INotifyPropertyChanged
    {
        private IComponentContext _components;

        private IInstallModpack _installModpack;
        private IDownloadClient _downloadClient;
        private IValidate _validate;

        public event PropertyChangedEventHandler PropertyChanged;

        public string DisplayName { get; set; }
        public string FilePath { get; set; }
        public string DownloadParameters { get; set; }

        public int CurrentDownloadProgress { get; set; }

        public bool IsIndeterminateProcess { get; set; }
        public bool IsModOrganizer { get; set; }
        public bool IsDownloading { get; set; }
        public bool IsValidationComplete { get; set; }

        public void Initialize(IComponentContext components)
        {
            _installModpack = components.Resolve<IInstallModpack>();
            _downloadClient = components.Resolve<IDownloadClient>();
            _validate = components.Resolve<IValidate>();

            _components = components;
        }

        public void InstallMod()
        {
            _installModpack.InstallMod(this);
        }

        public async Task InstallModAsync()
        {
            await Task.Run((Action)InstallMod);
        }

        public void DownloadMod()
        {
            _downloadClient.DownloadUpdate += (sender, args) =>
            {
                var downloadStatus = args as DownloadResponse;

                if (downloadStatus.DownloadStatus == DownloadStatus.Done)
                {
                    TryLoadArchive(downloadStatus.DownloadPath);
                }

                else if (downloadStatus.DownloadStatus == DownloadStatus.Downloading)
                {
                    CurrentDownloadProgress = downloadStatus.Percentage;
                }

                else if (downloadStatus.DownloadStatus == DownloadStatus.Failed)
                {
                    CurrentDownloadProgress = 0;
                }
            };

            _downloadClient.DownloadFile(this, DownloadParameters);
        }

        public async Task DownloadModAsync()
        {
            await Task.Run((Action)DownloadMod);
        }


        public bool TryLoadArchive(string archivePath)
        {
            var isArchiveMatch = _validate.IsArchiveMatch(this, archivePath);

            if (isArchiveMatch)
            {
                FilePath = archivePath;
            }

            return isArchiveMatch;
        }

        public async Task<bool> TryLoadArchiveAsync(string archivePath)
        {
            return await Task.Run(() => TryLoadArchive(archivePath));
        }


        public bool FindValidDirectoryArchive(List<string> directoryFiles)
        {
            var fileMatch = _validate.BatchFileMatch(this, directoryFiles);

            if (!string.IsNullOrEmpty(fileMatch))
            {
                FilePath = fileMatch;
            }

            return !string.IsNullOrEmpty(fileMatch);
        }

        public async Task<bool> FindValidDirectoryArchiveAsync(List<string> directoryFiles)
        {
            return await Task.Run(() => FindValidDirectoryArchive(directoryFiles));
        }
    }
}
