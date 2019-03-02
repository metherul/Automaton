using System;
using Autofac;
using Autofac.Core;
using Automaton.Model.Interfaces;
using Automaton.ViewModel.Content.Dialogs.Interfaces;
using Automaton.ViewModel.Content.Interfaces;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Interfaces;
using Automaton.ViewModel.Utilities.Interfaces;

namespace Automaton.ViewModel
{
    public class BootStrapper
    {
        private readonly ILifetimeScope _rootScope;

        public IViewModel MainWindow => Resolve<IMainWindowViewModel>();
        public IViewModel LoadModpack => Resolve<ILoadModpackViewModel>();
        public IViewModel InitialSetup => Resolve<IInitialSetupViewModel>();
        public IViewModel NexusLogin => Resolve<INexusLoginViewModel>();
        public IViewModel ValidateMods => Resolve<IValidateModsViewModel>();

        public IDialog GenericErrorDialog => Resolve<IGenericErrorDialog>();

        public IController DialogController => Resolve<IDialogController>();

        public BootStrapper()
        {
            var builder = new ContainerBuilder();
            var assembly = AppDomain.CurrentDomain.GetAssemblies();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => typeof(IUtility).IsAssignableFrom(t))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => typeof(IController).IsAssignableFrom(t))
                .SingleInstance()
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => typeof(IDialog).IsAssignableFrom(t))
                .SingleInstance()
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => typeof(IViewModel).IsAssignableFrom(t))
                .SingleInstance()
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => typeof(IService).IsAssignableFrom(t))
                .SingleInstance()
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => typeof(IModel).IsAssignableFrom(t))
                .AsImplementedInterfaces();

            _rootScope = builder.Build();
        }

        private T Resolve<T>()
        {
            return _rootScope.Resolve<T>(new Parameter[0]);
        }
    }
}
