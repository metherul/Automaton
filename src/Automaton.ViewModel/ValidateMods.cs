using System.Collections.ObjectModel;
using System.ComponentModel;
using Automaton.Model.ModpackBase;
using Automaton.ViewModel.Controllers;
using Automaton.Model.Utility;
using GalaSoft.MvvmLight.Command;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Automaton.ViewModel
{
    public class ValidateMods : ViewController, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Mod> MissingMods { get; set; }

        public RelayCommand<Mod> OpenModSourceUrlCommand { get; set; }
        public RelayCommand<Mod> FindAndValidateModFileCommand { get; set; }

        public string CurrentModName { get; set; }
        public string CurrentArchiveMd5 { get; set; }

        public int TotalSourceFileCount { get; set; }
        private int ThisViewIndex { get; } = 3;

        public bool InitialValidationComplete { get; set; }
        public bool AllModsValidated { get; set; }
        public bool IsComputeMd5 { get; set; }

        public ValidateMods()
        {
            OpenModSourceUrlCommand = new RelayCommand<Mod>(OpenModSourceUrl);
            FindAndValidateModFileCommand = new RelayCommand<Mod>(FindAndValidateModFile);

            Validation.ValidateSourcesUpdateEvent += ModValidationUpdate;
            ViewIndexChangedEvent += IncrementViewIndexUpdate;
        }

        private void OpenModSourceUrl(Mod currentMod)
        {
            Process.Start(currentMod.ModSourceUrl);
        }

        private async void FindAndValidateModFile(Mod currentMod)
        {
            var fileBrowser = new OpenFileDialog()
            {
                Title = $"Find {currentMod.ModName} | {currentMod.ModArchiveName}",
                InitialDirectory = "Downloads",
                Filter = "Mod Archive (*.zip;*.7zip;*.7z;*.rar;*.gzip)|*.zip;*.7zip;*.7z;*.rar;*.gzip",
            };

            if (fileBrowser.ShowDialog() == DialogResult.OK)
            {
                var archivePath = fileBrowser.FileName;
                var validationResponse = await Validation.ValidateModArchiveAsync(currentMod, archivePath);

                var test = MissingMods;

                MissingMods.Remove(currentMod);
            }

            // Show in UI
        }

        private void IncrementViewIndexUpdate(int currentIndex)
        {
            if (currentIndex == ThisViewIndex)
            {
                InitialValidation();
            }
        }

        private async void InitialValidation()
        {
            InitialValidationComplete = false;

            var sourceFiles = Validation.GetSourceFiles();
            TotalSourceFileCount = sourceFiles.Count;

            MissingMods = new ObservableCollection<Mod>(await Validation.ValidateSourcesAsync(sourceFiles));

            AllModsValidated = MissingMods.Count == 0;

            InitialValidationComplete = true;
        }

        private void ModValidationUpdate(Mod currentMod, bool isComputeMd5)
        {
            CurrentModName = currentMod.ModName;
            CurrentArchiveMd5 = currentMod.ArchiveMd5Sum;
            IsComputeMd5 = isComputeMd5;

        }
    }
}

