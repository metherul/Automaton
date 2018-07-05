using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Automaton.Controllers;
using Automaton.Model.Install;
using Automaton.Model.Modpack;

namespace Automaton.View.SetupSteps
{
    public class ValidationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Mod> MissingMods { get; set; } = new ObservableCollection<Mod>();

        public string TestString { get; set; }

        private int ThisViewIndex { get; } = 3;

        public ValidationViewModel()
        {
            ModValidation.ValidateSourcesUpdateEvent += ModValidationUpdate;

            ViewIndexController.ViewIndexChangedEvent += IncrementViewIndexUpdate;
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
            Generation.BuildInstallInstance();

            Task.Factory.StartNew(() => { MissingMods = new ObservableCollection<Mod>(ModValidation.ValidateSources()); });
        }

        private void ModValidationUpdate()
        {
            TestString = ModValidation.ModName;
            var test2 = ModValidation.IsComputeMd5;
        }

        private static void DisplayNewDebugLine(string line)
        {

        }
    }
}
