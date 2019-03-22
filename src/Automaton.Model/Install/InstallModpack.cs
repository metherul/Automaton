using Autofac;
using Automaton.Model.Install.Intefaces;

namespace Automaton.Model.Install
{
    public class InstallModpack
    {
        private readonly IInstallBase _installBase;

        public InstallModpack(IComponentContext components)
        {
            _installBase = components.Resolve<IInstallBase>();
        }

        public async void Install()
        {

        }
    }
}
