using System.Collections.ObjectModel;
using Autofac;
using Automaton.Model.Install.Intefaces;
using Automaton.Model.Modpack.Base;
using Automaton.ViewModel.Controllers;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;

namespace Automaton.ViewModel
{
    public class ValidateModsViewModel : ViewModelBase, IValidateModsViewModel
    {
        private readonly IViewController _viewController;
        private readonly IValidate _validate;

        public ObservableCollection<Mod> MissingMods { get; set; }

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
        }

        private async void ValidateMods()
        {
            await _validate.GetMissingModsAsync();
        }
    }
}
