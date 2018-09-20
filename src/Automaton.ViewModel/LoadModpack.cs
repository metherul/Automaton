using Automaton.Model.Utility;
using GalaSoft.MvvmLight.Command;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automaton.ViewModel
{
    public class LoadModpack
    {
        public RelayCommand LoadModpackCommand { get; set; }

        public LoadModpack()
        {
            LoadModpackCommand = new RelayCommand(Load);
        }

        private static void Load()
        {
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