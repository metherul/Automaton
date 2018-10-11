using Automaton.Model.ModpackBase;
using Automaton.Model.NexusApi;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Automaton.Model.ModpackBase.Interfaces;
using Automaton.Model.Utility.Interfaces;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;

namespace Automaton.ViewModel
{
    public class ValidateMods : INotifyPropertyChanged, IValidateMods
    {
        private readonly IViewController _viewController;
        private readonly IWindowNotificationController _windowNotificationController;
        private readonly IValidationUtilities _validateUtilities;
        private readonly IModpackUtilties _modpackUtilties;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<IMod> MissingMods { get; set; }

        public RelayCommand<IMod> OpenModSourceUrlCommand { get; set; }
        public RelayCommand<IMod> FindAndValidateModFileCommand { get; set; }
        public RelayCommand NexusLogInCommand { get; set; }
        public RelayCommand ContinueOfflineCommand { get; set; }

        public RelayCommand InstallModpackCommand { get; set; }

        public string CurrentModName { get; set; }
        public string CurrentArchiveMd5 { get; set; }
        public string LogInButtonText { get; set; } = "Nexus Login";

        public int TotalSourceFileCount { get; set; }
        private int ThisViewIndex { get; } = 3;

        public bool InitialValidationComplete { get; set; }
        public bool NoMissingMods { get; set; }

        public bool IsComputeMd5 { get; set; }
        public bool IsLoggedIn { get; set; }
        public bool IsLoginVisible { get; set; } = true;

        public ValidateMods(IViewController viewController, IWindowNotificationController windowNotificationController, IValidationUtilities validateUtilities,
                            IModpackUtilties modpackUtilties)
        {
            _viewController = viewController;
            _windowNotificationController = windowNotificationController;
            _validateUtilities = validateUtilities;
            _modpackUtilties = modpackUtilties;

            OpenModSourceUrlCommand = new RelayCommand<IMod>(OpenModSourceUrl);
            FindAndValidateModFileCommand = new RelayCommand<IMod>(FindAndValidateModFile);

            NexusLogInCommand = new RelayCommand(NexusLogIn);
            ContinueOfflineCommand = new RelayCommand(ContinueOffline);

            InstallModpackCommand = new RelayCommand(InstallModpack);

            _validateUtilities.ValidateSourcesUpdateEvent += ModValidationUpdate;
            _viewController.ViewIndexChangedEvent += IncrementViewIndexUpdate;
        }

        private void ContinueOffline()
        {
            IsLoggedIn = false;
            IsLoginVisible = false;
        }

        private void OpenModSourceUrl(IMod currentMod)
        {
            Process.Start(currentMod.ModSourceUrl);
        }

        private async void FindAndValidateModFile(IMod currentMod)
        {
            var fileBrowser = new OpenFileDialog()
            {
                Title = $"Find {currentMod.ModName} | {currentMod.FileName}",
                InitialDirectory = "Downloads",
                Filter = "Mod Archive (*.zip;*.7zip;*.7z;*.rar;*.gzip)|*.zip;*.7zip;*.7z;*.rar;*.gzip",
            };

            if (fileBrowser.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var archivePath = fileBrowser.FileName;
            var validationResult = false;

            MissingMods.Where(x => x == currentMod).First().IsIndeterminateProcess = true;

            await Task.Factory.StartNew(() =>
            {
                validationResult = _validateUtilities.IsMatchingModArchive(currentMod, archivePath).Result;
            });

            if (validationResult)
            {
                MissingMods.Where(x => x == currentMod).First().IsIndeterminateProcess = false;
                MissingMods.Remove(currentMod);

                NoMissingMods = MissingMods.Count == 0;
            }
            else
            {
                MissingMods.Where(x => x == currentMod).First().IsIndeterminateProcess = false;
            }
            // Show in UI
        }

        private void IncrementViewIndexUpdate(object sender, int currentIndex)
        {
            if (currentIndex == ThisViewIndex)
            {
                GetMissingMods();
            }
        }

        private void GetMissingMods()
        {
            InitialValidationComplete = false;

            var sourceFiles = _validateUtilities.GetSourceFiles();
            TotalSourceFileCount = sourceFiles.Count;

            MissingMods = new ObservableCollection<IMod>(_validateUtilities.GetMissingMods(sourceFiles));

            NoMissingMods = MissingMods.Count == 0;

            InitialValidationComplete = true;
        }

        private void ModValidationUpdate(IMod currentMod, bool isComputeMd5)
        {
            CurrentModName = currentMod.ModName;
            CurrentArchiveMd5 = currentMod.Md5;
            IsComputeMd5 = isComputeMd5;
        }

        /// <summary>
        /// Initializes the NXMWorker pipe listener.
        /// </summary>
        private void InitializeDownloadHandle()
        {
            if (NoMissingMods)
            {
                return;
            }

            var nexusProtocol = new NexusProtocol();

            // Start capturing piped messages from the NXMWorker, handle any progress reports.
            nexusProtocol.StartRecievingProtocolValues(new Progress<CaptureProtocolValuesProgress>(async x =>
            {
                var matchingMods = MissingMods.Where(y => y.NexusModId == x.ModId);
                
                if (!matchingMods.Any())
                {
                    return;
                }

                var matchingMod = matchingMods.First();

                if (matchingMod != null)
                {
                    _windowNotificationController.MoveToFront();

                    // Start downloading the mod file.
                    await NexusMod.DownloadModFile(matchingMod, x.FileId, new Progress<DownloadModFileProgress>(downloadProgress =>
                    {
                        MissingMods[MissingMods.IndexOf(matchingMod)].CurrentDownloadPercentage = downloadProgress.CurrentDownloadPercentage;

                        if (downloadProgress.IsDownloadComplete)
                        {
                            _modpackUtilties.UpdateModArchivePaths(matchingMod, downloadProgress.DownloadLocation);
                            MissingMods.Remove(matchingMod);

                            NoMissingMods = MissingMods.Count == 0;
                        }
                    }));
                }
            }));
        }

        private async void NexusLogIn()
        {
            await NexusConnection.StartNewConnectionAsync(new Progress<NewConnectionProgress>(x =>
            {
                if (!x.IsComplete)
                {
                    LogInButtonText = "Logging In";
                }

                if (x.HasError)
                {
                    LogInButtonText = "Nexus Login";
                }

                if (x.IsComplete)
                {
                    if (!x.HasError)
                    {
                        IsLoggedIn = true;
                        IsLoginVisible = false;

                        _windowNotificationController.MoveToFront();

                        InitializeDownloadHandle();
                    }
                    else
                    {
                        IsLoggedIn = false;
                        IsLoginVisible = true;
                    }
                }
            }));
        }

        private void InstallModpack()
        {
            _viewController.IncrementCurrentViewIndex();
        }
    }
}