using System.Collections.ObjectModel;
using System.ComponentModel;
using Automaton.Model.ModpackBase;
using Automaton.ViewModel.Controllers;
using Automaton.Model.Utility;

namespace Automaton.ViewModel
{
    public class ValidateMods : ViewController, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Mod> MissingMods { get; set; } = new ObservableCollection<Mod>();

        public string CurrentModName { get; set; }
        public string CurrentArchiveMd5 { get; set; }

        public int TotalSourceFileCount { get; set; }
        private int ThisViewIndex { get; } = 3;

        public bool InitialValidationComplete { get; set; }
        public bool AllModsValidated { get; set; }
        public bool IsComputeMd5 { get; set; }

        public ValidateMods()
        {
            Validation.ValidateSourcesUpdateEvent += ModValidationUpdate;
            ViewIndexChangedEvent += IncrementViewIndexUpdate;
        }

        private void IncrementViewIndexUpdate(int currentIndex)
        {
            if (currentIndex == ThisViewIndex)
            {
                InitializeInitValidation();
            }
        }

        private async void InitializeInitValidation()
        {
            InitialValidationComplete = false;

            var sourceFiles = Validation.GetSourceFiles();
            TotalSourceFileCount = sourceFiles.Count;

            var result = await Validation.ValidateSourcesAsync(sourceFiles);

            MissingMods = new ObservableCollection<Mod>(result);

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

