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
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Automaton.View.Resources.Bin.7z-x86.dll";

            if (Environment.Is64BitProcess)
            {
                resourceName = "Automaton.View.Resources.Bin.7z-x64.dll";
            }

            var stream = assembly.GetManifestResourceStream(resourceName);
            var dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z.dll");

            if (!File.Exists(dllPath))
            {
                var fileStream = File.Open(dllPath, FileMode.CreateNew);

                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
                stream.Dispose();

                fileStream.Close();
                fileStream.Dispose();
            }
        }
    }
}