using Automaton.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;

namespace Automaton.ViewModel
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Mod> ModsList { get; set; }

        public RelayCommand InstallModPackCommand { get; set; }
        public RelayCommand OpenIntialSetupCommand { get; set; }

        public string PackName { get; set; }
        public double GridHeight { get; set; }

        public MainWindowViewModel()
        {
            Messenger.Default.Register<double>(this, MessengerToken.WindowHeight, x => GridHeight = x - 103);
            Messenger.Default.Register<ModPack>(this, MessengerToken.FinalModPack, OnModPackUpdate);

            InstallModPackCommand = new RelayCommand(InstallModPack);
            OpenIntialSetupCommand = new RelayCommand(OpenInitialSetupDialog);

        }

        private void OnModPackUpdate(ModPack modPack)
        {
            ModsList = new ObservableCollection<Mod>(modPack.Mods);
            PackName = modPack.PackName;
        }

        private async void OpenInitialSetupDialog()
        {
            var view = new InitialSetupDialog();
            await DialogHost.Show(view, "RootDialog", OnMainDialogClosing);
        }

        private void OnMainDialogClosing(object sender, DialogClosingEventArgs eventargs)
        {
            PackHandler.GenerateFinalModPack();
        }

        private void InstallModPack()
        {
            var modPack = PackHandler.FinalModPack;
            var sourceLocation = PackHandler.SourceLocation;
            var installationLocation = PackHandler.InstallationLocation;

            PackHandler.InstallModPack(modPack, sourceLocation, installationLocation);
        }
    }
}
