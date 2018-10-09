using GalaSoft.MvvmLight.Command;
using System.Threading.Tasks;
using System.Windows.Forms;
using Automaton.Model.Utility.Interfaces;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;

namespace Automaton.ViewModel
{
    public class LoadModpack : ILoadModpack
    {
        private readonly IViewController _viewController;
        private readonly IThemeController _themeController;
        private readonly IModpackUtilties _modpackUtilties;

        public RelayCommand LoadModpackCommand { get; set; }

        public LoadModpack(IViewController viewController, IThemeController themeController, IModpackUtilties modpackUtilties)
        {
            _viewController = viewController;
            _themeController = themeController;
            _modpackUtilties = modpackUtilties;

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
                var loadModpackSuccess = false;

                Task.Factory.StartNew(() => 
                {
                    loadModpackSuccess = _modpackUtilties.LoadModpack(fileBrowser.FileName);
                }).Wait();

                if (loadModpackSuccess)
                {
                    _themeController.ApplyTheme();
                    _viewController.IncrementCurrentViewIndex();
                }
            }
        }
    }
}