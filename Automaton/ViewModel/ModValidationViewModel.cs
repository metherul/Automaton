using Automaton.Handles;
using Automaton.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace Automaton.ViewModel
{
    class ModValidationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand ValidateCommand { get; set; }
        public RelayCommand OpenModLinkCommand { get; set; }
        public RelayCommand<object> ViewModInfoCommand { get; set; }

        public List<Mod> MissingMods { get; set; }

        public string ModName { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public string ModLink { get; set; }
        public string ModJson { get; set; }

        public string LinkType { get; set; }
        public string UtilityButton { get; set; }
        public string UtilityButtonIcon { get; set; }

        public bool IsValidationComplete { get; set; }

        public ModValidationViewModel()
        {
            ValidateCommand = new RelayCommand(Validate);
            OpenModLinkCommand = new RelayCommand(OpenModLink);
            ViewModInfoCommand = new RelayCommand<object>((x) => ViewModInfo(x));

            LinkType = "LinkVariantOff";
            UtilityButton = "RESCAN SOURCE MODS FOLDER";
            UtilityButtonIcon = "Sync";

            IsValidationComplete = false;

            Messenger.Default.Register<List<Mod>>(this, MessengerToken.MissingMods, x => MissingMods = x);
        }

        private void Validate()
        {
            var missingMods = PackHandler.ValidateSourceLocation();

            if (missingMods.Count == 0)
            {
                UtilityButton = "NEXT";
                UtilityButtonIcon = "ArrowRight";

                ModName = "";
                FileName = "";
                FileSize = "";
                ModJson = "";
                ModLink = "";

                IsValidationComplete = true;

                ValidateCommand = new RelayCommand(NextCard);
            }
        }

        private void NextCard()
        {
            TransitionHandler.CalculateNextCard(CardIndex.InitialSetup);
        }

        private void OpenModLink()
        {
            if (!string.IsNullOrEmpty(ModLink))
            {
                Process.Start(ModLink);
            }
        }

        private void ViewModInfo(object parameter)
        {
            try
            {
                var targetMod = (Mod)((parameter as MouseButtonEventArgs).OriginalSource as dynamic).DataContext;

                ModName = targetMod.ModName;
                FileName = targetMod.FileName;
                FileSize = $"{targetMod.FileSize} Bytes";
                ModLink = targetMod.ModLink;
                ModJson = JsonConvert.SerializeObject(targetMod, Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                if (string.IsNullOrEmpty(ModLink))
                {
                    LinkType = "LinkVariantOff";
                }

                else
                {
                    LinkType = "LinkVariant";
                }
            }

            catch
            {
                return;
            }


        }
    }
}
