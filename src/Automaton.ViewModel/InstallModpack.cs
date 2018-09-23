using Automaton.Model.Utility;
using Automaton.ViewModel.Controllers;
using Automaton.ViewModel.Interfaces;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Automaton.ViewModel
{
    public class InstallModpack : ViewController, IInstallModpack, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string ConsoleOut { get; set; }

        private int ThisViewIndex { get; } = 4;

        public InstallModpack()
        {
            ViewIndexChangedEvent += IncrementViewIndexUpdate;
        }

        private void IncrementViewIndexUpdate(int index)
        {
            if (index == ThisViewIndex)
            {
                StartModpackInstall();
            }
        }

        private void StartModpackInstall()
        {
            Task.Factory.StartNew(() =>
            {
                Modpack.InstallModpack(new Progress<InstallModpackProgress>(installProgress =>
                {
                    ConsoleOut += installProgress.UpdateString + "\n";
                }));
            });
        }
    }
}
