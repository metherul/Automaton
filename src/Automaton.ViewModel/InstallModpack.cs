using Automaton.Model.Utility;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Automaton.Model.Utility.Interfaces;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;

namespace Automaton.ViewModel
{
    public class InstallModpack : IInstallModpack, INotifyPropertyChanged
    {
        private readonly IModpackUtilties _modpackUtilties;

        public event PropertyChangedEventHandler PropertyChanged;

        public string ConsoleOut { get; set; }

        private int ThisViewIndex { get; } = 4;

        public InstallModpack(IViewController viewController, IModpackUtilties modpackUtilties)
        {
            _modpackUtilties = modpackUtilties;

            viewController.ViewIndexChangedEvent += IncrementViewIndexUpdate;
        }

        private void IncrementViewIndexUpdate(object sender, int currentIndex)
        {
            if (currentIndex == ThisViewIndex)
            {
                StartModpackInstall();
            }
        }

        private void StartModpackInstall()
        {
            Task.Factory.StartNew(() =>
            {
                _modpackUtilties.InstallModpack(new Progress<InstallModpackProgress>(installProgress =>
                {
                    ConsoleOut += installProgress.UpdateString + "\n";
                }));
            });
        }
    }
}
