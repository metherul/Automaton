using System;
using System.Diagnostics;
using Automaton.ViewModel.Interfaces;
using GalaSoft.MvvmLight.Command;

namespace Automaton.ViewModel
{
    public class FinishedInstallViewModel : IFinishedInstallViewModel
    {
        public RelayCommand OpenPatreonCommand => new RelayCommand(OpenPatreon);

        private void OpenPatreon()
        {
            Process.Start("https://www.patreon.com/metherul");
        }
    }
}
