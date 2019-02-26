using Automaton.Model.Utility;
using Automaton.ViewModel.Controllers;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Automaton.ViewModel
{
    public class InstallModpack : ViewController, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string ConsoleOut { get; set; }

        private int ThisViewIndex { get; } = 3;

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
