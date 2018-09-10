﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using Automaton.Model.ModpackBase;
using Automaton.ViewModel.Controllers;
using Automaton.Model.Utility;
using GalaSoft.MvvmLight.Command;
using System.Diagnostics;
using System.Windows.Forms;
using System;
using Automaton.Model.NexusApi;

namespace Automaton.ViewModel
{
    public class ValidateMods : ViewController, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Mod> MissingMods { get; set; }

        public RelayCommand<Mod> OpenModSourceUrlCommand { get; set; }
        public RelayCommand<Mod> FindAndValidateModFileCommand { get; set; }
        public RelayCommand NexusLogInCommand { get; set; }
        public RelayCommand ContinueOfflineCommand { get; set; }

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

        public ValidateMods()
        {
            OpenModSourceUrlCommand = new RelayCommand<Mod>(OpenModSourceUrl);
            FindAndValidateModFileCommand = new RelayCommand<Mod>(FindAndValidateModFile);
            NexusLogInCommand = new RelayCommand(NexusLogIn);
            ContinueOfflineCommand = new RelayCommand(ContinueOffline);

            Validation.ValidateSourcesUpdateEvent += ModValidationUpdate;
            ViewIndexChangedEvent += IncrementViewIndexUpdate;
        }

        private void ContinueOffline()
        {
            IsLoggedIn = false;
            IsLoginVisible = false;
        }

        private void OpenModSourceUrl(Mod currentMod)
        {
            Process.Start(currentMod.ModSourceUrl);
        }

        private void FindAndValidateModFile(Mod currentMod)
        {
            var fileBrowser = new OpenFileDialog()
            {
                Title = $"Find {currentMod.ModName} | {currentMod.ModArchiveName}",
                InitialDirectory = "Downloads",
                Filter = "Mod Archive (*.zip;*.7zip;*.7z;*.rar;*.gzip)|*.zip;*.7zip;*.7z;*.rar;*.gzip",
            };

            if (fileBrowser.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var archivePath = fileBrowser.FileName;
            var validationResponse = Validation.IsMatchingModArchive(currentMod, archivePath);

            if (validationResponse)
            {
                MissingMods.Remove(currentMod);
            }
            // Show in UI
        }

        private void IncrementViewIndexUpdate(int currentIndex)
        {
            if (currentIndex == ThisViewIndex)
            {
                GetMissingMods();
            }
        }

        private void GetMissingMods()
        {
            InitialValidationComplete = false;

            var sourceFiles = Validation.GetSourceFiles();
            TotalSourceFileCount = sourceFiles.Count;

            MissingMods = new ObservableCollection<Mod>(Validation.GetMissingMods(sourceFiles));

            NoMissingMods = MissingMods.Count == 0;

            InitialValidationComplete = true;
        }

        private void ModValidationUpdate(Mod currentMod, bool isComputeMd5)
        {
            CurrentModName = currentMod.ModName;
            CurrentArchiveMd5 = currentMod.ArchiveMd5Sum;
            IsComputeMd5 = isComputeMd5;

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
                    }

                    else
                    {
                        IsLoggedIn = false;
                        IsLoginVisible = true;
                    }
                }

            }));
        }
    }
}

