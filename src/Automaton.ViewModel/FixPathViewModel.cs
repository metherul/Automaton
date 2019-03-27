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
        private readonly IPathFix _fixPath;
        private readonly ILogger _logger;

        public RelayCommand StartFixPathCommand => new RelayCommand(StartFixPath);

        private bool IsVisible { get; set; }

        public FixPathViewModel(IComponentContext components)
        {
            _viewController = components.Resolve<IViewController>();
            _fixPath = components.Resolve<IPathFix>();
            _logger = components.Resolve<ILogger>();

            var isLongPathEnabled = _fixPath.IsLongPathsEnabled();

            if (!isLongPathEnabled)
            {
                IsVisible = true;

                _logger.WriteLine("Long path is disabled. Waiting for user response...");
            }

            else
            {
                _viewController.IncrementCurrentViewIndex();

                _logger.WriteLine("Long path enabled. Continuing.");
            }
        }

        private void StartFixPath()
        {
            _logger.WriteLine("Attempting path length patch...");

            var fixResult = _fixPath.EnableLongPaths();

            if (fixResult)
            {
                _viewController.IncrementCurrentViewIndex();

                _logger.WriteLine("Path length resolved.");
            }

            _logger.WriteLine("Path length patch failed.");
        }
    }
}
