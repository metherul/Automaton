using Automaton.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Automaton.ViewModel
{
    class ModValidationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand ValidateCommand { get; set; }

        public List<Mod> MissingMods { get; set; }

        public string ModName { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public string ModLink { get; set; }
        public string ModJson { get; set; }

        public ModValidationViewModel()
        {
            ValidateCommand = new RelayCommand(Validate);

            ModName = "null";
            FileName = "null";
            FileSize = "null";
            ModLink = "null";

            Messenger.Default.Register<List<Mod>>(this, MessengerToken.MissingMods, x => MissingMods = x);
        }

        public void Validate()
        {
            ModJson = File.ReadAllText(@"C:\Programming\C# Projects\Automaton\Automaton\bin\Debug\modpack.json");
        }
    }
}
