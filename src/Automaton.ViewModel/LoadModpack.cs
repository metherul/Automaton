using System.ComponentModel;
using Automaton.Model.Utility;
using GalaSoft.MvvmLight.Command;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automaton.ViewModel
{
    public class LoadModpack : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand LoadModpackCommand { get; set; }

        public bool IsLoadingModpack { get; set; } = false;

        public LoadModpack()
        {
            LoadModpackCommand = new RelayCommand(Load);
        }

        private void Load()
        {
            IsLoadingModpack = true;

            var fileBrowser = new OpenFileDialog()
            {
                Title = "Choose an Automaton Modpack",
                InitialDirectory = "Downloads",
                Filter = "Modpack Files (*.zip;*.auto)|*.zip;*.auto",
            };

            if (fileBrowser.ShowDialog() == DialogResult.OK)
            {
                Task.Factory.StartNew(() => { Modpack.LoadModpack(fileBrowser.FileName); });
            }
        }
    }
}