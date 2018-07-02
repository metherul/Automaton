using System.Threading.Tasks;
using System.Windows.Forms;
using Automaton.Model.Modpack;
using GalaSoft.MvvmLight.Command;

namespace Automaton.View.SetupSteps
{
    public class LoadModpackViewModel
    {
        public RelayCommand LoadModpackCommand { get; set; }

        public LoadModpackViewModel()
        {
            LoadModpackCommand = new RelayCommand(LoadModpack);
        }

        private static void LoadModpack()
        {
            var fileBrowser = new OpenFileDialog()
            {
                Title = "Choose an Automaton Modpack",
                InitialDirectory = "Downloads",
                Filter = "Modpack Files (*.zip;*.auto)|*.zip;*.auto",
            };

            if (fileBrowser.ShowDialog() == DialogResult.OK)
            {
                Task.Factory.StartNew(() => { ModpackUtilities.LoadModpack(fileBrowser.FileName); });
            }
        }
    }
}
