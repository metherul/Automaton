using Automaton.Model;
using Automaton.Model.Utility;
using System;
using System.Linq;
using System.Windows;

namespace Automaton.View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e != null // Check if args are null
                && e.Args.Any() // Check if args contain any data
                && e.Args[0].StartsWith("nxm", StringComparison.OrdinalIgnoreCase) // Check to see if it contains correct data
                && ProcessFinder.IsProcessAlreadyRunning()) // Check to see if the Automaton process is already running
            {
                NamedPipes.SendMessage(e.Args[0]);
            }
        }
    }
}