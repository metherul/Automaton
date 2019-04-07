using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Automaton.Model.Interfaces;
using Automaton.Model.NexusApi;
using Automaton.Model.NexusApi.Interfaces;
using Automaton.ViewModel.Dialogs.Interfaces;
using Automaton.ViewModel.Interfaces;
using Automaton.ViewModel.Controllers.Interfaces;
using Automaton.ViewModel.Utilities.Interfaces;
using Application = System.Windows.Forms.Application;

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

            // For performance, the NXM router will initialize and run its checks here.
            if (Process.GetProcessesByName("Automaton").Length > 1)
            {
                var cliArgs = Environment.GetCommandLineArgs();

                if (cliArgs.Length == 2 && cliArgs[0] == Assembly.GetEntryAssembly().Location)
                {
                    // Build the builder with the router ONLY and pipe the needed data.
                    builder.RegisterAssemblyTypes(assembly)
                        .Where(t => typeof(INxmHandle).IsAssignableFrom(t))
                        .SingleInstance()
                        .AsImplementedInterfaces();
                    _rootScope = builder.Build();

                    var nxmHandle = Resolve<INxmHandle>();

                    nxmHandle.ConnectClient();
                    nxmHandle.SendClientMessage(new PipedData(cliArgs[1]));
                }

                // Kill the app, we do not want more than one instance of Automaton floating around.
                Application.Exit();
            }

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
