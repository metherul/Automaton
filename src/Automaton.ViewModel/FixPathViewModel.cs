using Autofac;
using Automaton.Model.Interfaces;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;
using GalaSoft.MvvmLight.Command;
using System.Security.Permissions;

namespace Automaton.ViewModel
{
    public class FixPathViewModel : ViewModelBase, IFixPathViewModel
    {
        private readonly IViewController _viewController;
        private readonly IRegistryHandle _registryHandle;

        public RelayCommand StartFixPathCommand => new RelayCommand(StartFixPath);
        public RelayCommand SkipStepCommand => new RelayCommand(SkipStep);

        public FixPathViewModel(IComponentContext components)
        {
            _viewController = components.Resolve<IViewController>();
            _registryHandle = components.Resolve<IRegistryHandle>();

            _registryHandle = _registryHandle.New(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\FileSystem", "LongPathsEnabled");

            if ((int)_registryHandle.GetValue() == 1)
            {
                _viewController.IncrementCurrentViewIndex();
            }
        }

        private void StartFixPath()
        {
            // Attempt to apply the fix here
            _registryHandle.SetValue(1);

            if ((int)_registryHandle.GetValue() == 1)
            {
                _viewController.IncrementCurrentViewIndex();
            }
        }

        private void SkipStep()
        {
            _viewController.IncrementCurrentViewIndex();
        }
    }
}
