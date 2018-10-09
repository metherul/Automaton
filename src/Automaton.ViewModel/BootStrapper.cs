using Autofac;
using Autofac.Core;
using Automaton.Model.Interfaces;
using System.Reflection;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;

namespace Automaton.ViewModel
{
    public class BootStrapper
    {
        private readonly ILifetimeScope _rootScope;

        public IViewModel MainWindow => Resolve<IMainWindow>();
        public IViewModel LoadModpack => Resolve<ILoadModpack>();
        public IViewModel InitialSetup => Resolve<IInitialSetup>();
        public IViewModel SetupAssistant => Resolve<ISetupAssistant>();
        public IViewModel ValidateMods => Resolve<IValidateMods>();
        public IViewModel InstallModpack => Resolve<IInstallModpack>();

        public BootStrapper()
        {
            var builder = new ContainerBuilder();
            var assembly = Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => typeof(IModel).IsAssignableFrom(t))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => typeof(IController).IsAssignableFrom(t))
                .SingleInstance()
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assembly)
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
