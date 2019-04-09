using Automaton.Model.Modpack.Base;
using System.ComponentModel;

namespace Automaton.Model.Install
{
    public class ExtendedMod : Mod, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string DisplayName { get; set; }
        public string FilePath { get; set; }

        public int CurrentDownloadProgress { get; set; }

        public bool IsIndeterminateProcess { get; set; }
        public bool IsModOrganizer { get; set; }
        public bool IsDownloading { get; set; }
    }
}
