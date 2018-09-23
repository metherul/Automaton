using Autofac;
using Autofac.Core;
using Automaton.Model.Interfaces;
using Automaton.ViewModel.Interfaces;
using System.Linq;
using System.Reflection;

namespace Automaton.ViewModel
{
    public class BootStrapper
    {
        private ILifetimeScope _rootScope;

        public IViewModel MainWindow
        {
            get
            {
                return Resolve<IMainWindow>();
            }
        }
        public IViewModel LoadModpack
        {
            get
            {
                return Resolve<ILoadModpack>();
            }
        }
        public IViewModel InitialSetup
        {
            get
            {
                return Resolve<IInitialSetup>();
            }
        }
        public IViewModel SetupAssistant
        {
            get
            {
                return Resolve<ISetupAssistant>();
            }
        }
        public IViewModel ValidateMods
        {
            get
            {
                return Resolve<IValidateMods>();
            }
        }
        public IViewModel InstallModpack
        {
            get
            {
                return Resolve<IInstallModpack>();
            }
        }

        public BootStrapper()
        {
            var builder = new ContainerBuilder();
            var assemblies = new[] { Assembly.GetExecutingAssembly() };

            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => typeof(IModel).IsAssignableFrom(t))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => typeof(IViewModel).IsAssignableFrom(t))
                .AsImplementedInterfaces();

            _rootScope = builder.Build();
        }

        private T Resolve<T>()
        {
            return _rootScope.Resolve<T>(new Parameter[0]);
        }
    }
}
