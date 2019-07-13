using Automaton.Model;
using System;
using System.IO;
using System.Reflection;
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
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Ssl3 | System.Net.SecurityProtocolType.Tls11;

            var resourceDictionary = Application.Current.Resources;
            resourceDictionary["AutomatonVersion"] = Assembly.GetEntryAssembly().GetName().Version.ToString();
        }
    }
}