using Automaton.Model.Utility;
using Automaton.ViewModel.Controllers;
using GalaSoft.MvvmLight.Command;
using System.Threading.Tasks;
using System.Windows.Forms;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;

namespace Automaton.ViewModel
{
    public class LoadModpack : ILoadModpack
    {
        private IViewController _viewController;

        public RelayCommand LoadModpackCommand { get; set; }

        public LoadModpack(IViewController viewController)
        {
            _viewController = viewController;

            LoadModpackCommand = new RelayCommand(Load);
        }

        private void Load()
        {
            var fileBrowser = new OpenFileDialog()
            {
                Title = "Choose an Automaton Modpack",
                InitialDirectory = "Downloads",
                Filter = "Modpack Files (*.zip;*.auto)|*.zip;*.auto",
            };

            if (fileBrowser.ShowDialog() == DialogResult.OK)
            {
                Task.Factory.StartNew(() => 
                {
                    Modpack.LoadModpack(fileBrowser.FileName);
                }).Wait();
            }

            _viewController.IncrementCurrentViewIndex();
        }
    }
}