using Automaton.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace Automaton.ViewModel
{
    class ModValidationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand ValidateCommand { get; set; }
        public RelayCommand<object> ViewModInfoCommand { get; set; }

        public List<Mod> MissingMods { get; set; }

        public string ModName { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public string ModLink { get; set; }
        public string ModJson { get; set; }

        public ModValidationViewModel()
        {
            ValidateCommand = new RelayCommand(Validate);
            ViewModInfoCommand = new RelayCommand<object>((x) => ViewModInfo(x));

            ModName = "null";
            FileName = "null";
            FileSize = "null";
            ModLink = "null";

            Messenger.Default.Register<List<Mod>>(this, MessengerToken.MissingMods, x => MissingMods = x);
        }

        public void Validate()
        {

        }

        public void ViewModInfo(object parameter)
        {
            var targetMod = (Mod)((parameter as MouseButtonEventArgs).OriginalSource as dynamic).DataContext;

            ModName = targetMod.ModName;
            FileName = targetMod.FileName;
            FileSize = $"{targetMod.FileSize} Bytes";
            ModLink = "-";
            ModJson = JsonConvert.SerializeObject(targetMod, Formatting.Indented,
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }
    }
}
