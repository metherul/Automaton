using Autofac;
using Automaton.Model.Install.Intefaces;
using Automaton.Model.Install.Interfaces;
using System;

namespace Automaton.Model.Install
{
    public class InstallModpack : IInstallModpack
    {
        private readonly IInstallBase _installBase;

        public EventHandler<string> DebugLogCallback { get; set; }

        public InstallModpack(IComponentContext components)
        {
            _installBase = components.Resolve<IInstallBase>();
        }

        public void Install()
        {
            for (int i = 0; i <= 10; i++)
            {
                DebugLogCallback.Invoke(this, "test" + i);
            }
        }
    }
}
