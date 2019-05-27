using Autofac;
using Automaton.Model.Interfaces;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;
using GalaSoft.MvvmLight.Command;

namespace Automaton.ViewModel
{
    public class FixPathViewModel : ViewModelBase, IFixPathViewModel
    {
        private readonly IViewController _viewController;

        public RelayCommand StartFixPathCommand => new RelayCommand(StartFixPath);

        private bool IsVisible { get; set; }

        public FixPathViewModel(IComponentContext components)
        {
            _viewController = components.Resolve<IViewController>();
        }

        private void StartFixPath()
        {
        }
    }
}
