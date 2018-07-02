using System.Collections.ObjectModel;
using System.ComponentModel;
using Automaton.Controllers;
using Automaton.Model.Install;
using Automaton.Model.Modpack;

namespace Automaton.View.SetupSteps
{
    public class ValidationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Mod> MissingMods { get; set; } = new ObservableCollection<Mod>();

        private int ThisViewIndex { get; } = 3;

        public ValidationViewModel()
        {
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

            MissingMods = new ObservableCollection<Mod>(ModValidation.ValidateSources());
        }

        private static void DisplayNewDebugLine(string line)
        {

        }
    }
}
