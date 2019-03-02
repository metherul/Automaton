using System.Collections.ObjectModel;
using System.Diagnostics;
using Autofac;
using Automaton.Model.Install;
using Automaton.Model.Install.Intefaces;
using Automaton.ViewModel.Controllers;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;
using GalaSoft.MvvmLight.Command;

namespace Automaton.ViewModel
{
    public class ValidateModsViewModel : ViewModelBase, IValidateModsViewModel
    {
        private readonly IViewController _viewController;
        private readonly IValidate _validate;

        public RelayCommand FindAndValidateModFileCommand { get; set; }
        public RelayCommand OpenModSourceUrlCommand { get; set; }

        public ObservableCollection<ExtendedMod> MissingMods { get; set; } = new ObservableCollection<ExtendedMod>();

        public ValidateModsViewModel(IComponentContext components)
        {
            _viewController = components.Resolve<IViewController>();
            _validate = components.Resolve<IValidate>();

            _viewController.ViewIndexChangedEvent += ViewControllerOnViewIndexChangedEvent;
        }

        private void ViewControllerOnViewIndexChangedEvent(object sender, int e)
        {
            if (e != (int)ViewIndex.ValidateMods)
            {
                return;
            }

            ValidateMods();
        }

        private async void ValidateMods()
        {
            var missingMods = await _validate.GetMissingModsAsync();

            Debug.WriteLine("Finished analysis");
        }
    }
}
