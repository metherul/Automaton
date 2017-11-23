using Automaton.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.ComponentModel;

namespace Automaton.ViewModel
{
    class SetLocationsDialogViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand GetSourceLocationCommand { get; set; }
        public RelayCommand GetInstallationLocationCommand { get; set; }
        public RelayCommand NextButtonPressedCommand { get; set; }
        public RelayCommand CloseMainDialogCommand { get; set; }

        public string SourceLocation { get; set;}
        public string InstallationLocation { get; set; }

        public SetLocationsDialogViewModel()
        {
            GetSourceLocationCommand = new RelayCommand(BrowseSourceLocation);
            GetInstallationLocationCommand = new RelayCommand(BrowseInstallationLocation);
            NextButtonPressedCommand = new RelayCommand(NextButtonPressed);
            CloseMainDialogCommand = new RelayCommand(CloseMainDialog);
        }

        private void BrowseSourceLocation()
        {
            var dialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                Title = "Select the mod source location."
            };

            var result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                SourceLocation = dialog.FileName;
            }
        }

        private void BrowseInstallationLocation()
        {
            var dialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                Title = "Select an install location."
            };

            var result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                InstallationLocation = dialog.FileName;
            }
        }

        private void NextButtonPressed()
        {
            PackHandler.SourceLocation = SourceLocation;
            PackHandler.InstallationLocation = InstallationLocation;

            CloseMainDialog();

            if (PackHandler.ModPack.OptionalInstallation != null && PackHandler.ModPack.OptionalInstallation.Groups.Count > 0)
            {
                var window = new OptionalsInstaller();
                window.ShowDialog();
            }

            PackHandler.GenerateFinalModPack(PackHandler.ModPack);
            PackHandler.ValidateSourceLocation(PackHandler.SourceLocation);
        }

        private void CloseMainDialog()
        {
            Messenger.Default.Send(false, MessengerToken.CloseMainDialog);
        }
    }
}
