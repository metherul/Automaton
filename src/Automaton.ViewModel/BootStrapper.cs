using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Automaton.ViewModel.Dialogs.Interfaces;
using Automaton.ViewModel.Interfaces;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Utilities.Interfaces;
using Automaton.Model.Interfaces;

namespace Automaton.ViewModel
{
    public class BootStrapper
    {
        private readonly ILifetimeScope _rootScope;

        public IViewModel MainWindow => Resolve<IMainWindowViewModel>();
        public IViewModel FixPath => Resolve<IFixPathViewModel>();
        public IViewModel LoadModpack => Resolve<ILoadModpackViewModel>();
        public IViewModel InitialSetup => Resolve<IInitialSetupViewModel>();
        public IViewModel NexusLogin => Resolve<INexusLoginViewModel>();
        public IViewModel ValidateMods => Resolve<IValidateModsViewModel>();
        public IViewModel InstallModpack => Resolve<IInstallModpackViewModel>();
        public IViewModel FinishedInstall => Resolve<IFinishedInstallViewModel>();

        public IDialog GenericErrorDialog => Resolve<IGenericErrorDialog>();
        public IDialog GenericLogDialog => Resolve<IGenericLogDialog>();

        public IController DialogController => Resolve<IDialogController>();

        public BootStrapper()
        {
            var builder = new ContainerBuilder();
            var assembly = AppDomain.CurrentDomain.GetAssemblies();

            // Initialize the NXM router here

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
                .Where(t => typeof(IModel).IsAssignableFrom(t))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => typeof(ISingleton).IsAssignableFrom(t))
                .SingleInstance()
                .AsImplementedInterfaces();

            _rootScope = builder.Build();
        }

        private T Resolve<T>()
        {
            return _rootScope.Resolve<T>(new Parameter[0]);
        }
    }
}
