using System.Threading.Tasks;
using Automaton.ViewModel.Utilities.Interfaces;
using Ookii.Dialogs.Wpf;

namespace Automaton.ViewModel.Utilities
{
    public class FileSystemBrowser : IFileSystemBrowser
    {
        public async Task<string> OpenFileBrowserAsync(string filter, string windowTitle)
        {
            return await Task.Run(() => OpenFileBrowser(filter, windowTitle));
        }

        public async Task<string> OpenDirectoryBrowserAsync(string windowTitle)
        {
            return await Task.Run(() => OpenDirectoryBrowser(windowTitle));
        }

        public string OpenFileBrowser(string filter, string windowTitle)
        {
            var dialog = new VistaOpenFileDialog()
            {
                Filter = filter,
                Title = windowTitle
            };

            if (dialog.ShowDialog() ?? false)
            {
                return dialog.FileName;
            }

            return "";
        }

        public string OpenDirectoryBrowser(string windowTitle)
        {
            var dialog = new VistaFolderBrowserDialog()
            {
                ShowNewFolderButton = true,
                Description = windowTitle,
                UseDescriptionForTitle = true
            };

            if (dialog.ShowDialog() ?? false)
            {
                return dialog.SelectedPath;
            }

            return "";
        }
    }
}
