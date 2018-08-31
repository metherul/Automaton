using System.Collections.ObjectModel;
using System.ComponentModel;
using Automaton.Model.ModpackBase;
using Automaton.ViewModel.Controllers;
using Automaton.Model.Utility;
using GalaSoft.MvvmLight.Command;

namespace Automaton.ViewModel
{
    public class ValidateMods : ViewController, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Mod> MissingMods { get; set; } 

        public RelayCommand<dynamic> OpenModSourceUrlCommand { get; set; }

        public string CurrentModName { get; set; }
        public string CurrentArchiveMd5 { get; set; }

        public int TotalSourceFileCount { get; set; }
        private int ThisViewIndex { get; } = 3;

        public bool InitialValidationComplete { get; set; }
        public bool AllModsValidated { get; set; }
        public bool IsComputeMd5 { get; set; }

        public ValidateMods()
        {
            OpenModSourceUrlCommand = new RelayCommand<dynamic>(OpenModSourceUrl);

            Validation.ValidateSourcesUpdateEvent += ModValidationUpdate;
            ViewIndexChangedEvent += IncrementViewIndexUpdate;
        }

        private void OpenModSourceUrl(dynamic currentMod)
        {

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

