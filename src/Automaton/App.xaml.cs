using Alphaleonis.Win32.Filesystem;
using System;
using System.Runtime.ExceptionServices;
using System.Windows;

namespace Automaton
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var loggingPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");

            if (!File.Exists(loggingPath))
            {
                File.Create(loggingPath).Dispose();
            }

            File.AppendAllText(loggingPath, $"{Environment.NewLine}{DateTime.Now} - Automaton Initiated");

            AppDomain.CurrentDomain.FirstChanceException += (object source, FirstChanceExceptionEventArgs e) =>
            {
                File.AppendAllText(loggingPath, $"{Environment.NewLine}{DateTime.Now} - {e.Exception.StackTrace} {Environment.NewLine}--> {e.Exception.Message}");
            };

        }
    }
}
