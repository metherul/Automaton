using Automaton.Handles;
using Automaton.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Automaton.ViewModel
{
    class ModListViewerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Mod> ModsList { get; set; }

        public RelayCommand InstallModPackCommand { get; set; }

        public string PackName { get; set; }

        public ModListViewerViewModel()
        {
            Messenger.Default.Register<ModPack>(this, MessengerToken.ModPack, OnModPackUpdate);

            InstallModPackCommand = new RelayCommand(InstallModPack);
        }

        private void OnModPackUpdate(ModPack modPack)
        {
            ModsList = new ObservableCollection<Mod>(modPack.Mods);
            PackName = modPack.PackName;
        }

        private void InstallModPack()
        {
            TransitionHandler.CalculateNextCard(CardIndex.ModViewer);

            PackHandler.ThreadedInstallModPack();
        }
    }
}
