using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Data;
using Automaton.ViewModel.Controllers;
using Automaton.Model.Utility;

namespace Automaton.ViewModel
{
    public class InitialValidation : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Mod> MissingMods { get; set; } = new ObservableCollection<Mod>();

        public string CurrentModName { get; set; }
        public string CurrentArchiveMd5 { get; set; }

        public int TotalSourceFileCount { get; set; }
        private int ThisViewIndex { get; } = 3;

        public bool InitialValidationComplete { get; set; } = false;
        public bool AllModsValidated { get; set; } = false;

        public InitialValidation()
        {
            Validation.ValidateSourcesUpdateEvent += ModValidationUpdate;

            ViewController.ViewIndexChangedEvent += IncrementViewIndexUpdate;
        }

        private void IncrementViewIndexUpdate(int currentIndex)
        {
            if (currentIndex == ThisViewIndex)
            {
                InitializeInitValidation();
            }
        }

        private void InitializeInitValidation()
        {
            Task.Factory.StartNew(() =>
            {
                var sourceFiles = Validation.GetSourceFiles();

                TotalSourceFileCount = sourceFiles.Count;

                MissingMods = new ObservableCollection<Mod>(Validation.ValidateSources(sourceFiles));
            });

            AllModsValidated = MissingMods.Count == 0;
        }

        private void ModValidationUpdate()
        {
            CurrentModName = Validation.CurrentMod.ModName;
            CurrentArchiveMd5 = Validation.CurrentMod.ArchiveMd5Sum;

            InitialValidationComplete = Validation.IsComplete;
        }
    }

    public class BoolToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? 1 : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
