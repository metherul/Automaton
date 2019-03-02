using System.Collections.ObjectModel;
using Autofac;
using Automaton.Model.Install.Intefaces;
using Automaton.Model.Modpack.Base;
using Automaton.ViewModel.Interfaces;

namespace Automaton.ViewModel
{
    public class ValidateModsViewModel : ViewModelBase, IValidateModsViewModel
    {
        private readonly IValidate _validate;

        public ObservableCollection<Mod> MissingMods { get; set; }

        public ValidateModsViewModel(IComponentContext components)
        {
            _validate = components.Resolve<IValidate>();
        }
    }
}
