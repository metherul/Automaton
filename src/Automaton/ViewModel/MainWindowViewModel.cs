using Automaton.Model;
using GalaSoft.MvvmLight.Command;

namespace Automaton.ViewModel
{
    class MainWindowViewModel
    {
        public RelayCommand OnWindowCloseCommand { get; set; }

        public MainWindowViewModel()
        {
            OnWindowCloseCommand = new RelayCommand(OnWindowClose);
        }

        private void OnWindowClose()
        {
            ApplicationClose.Cleanup();
        }
    }
}
