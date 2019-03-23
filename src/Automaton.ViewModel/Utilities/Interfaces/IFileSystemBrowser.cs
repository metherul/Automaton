using System.Threading.Tasks;

namespace Automaton.ViewModel.Utilities.Interfaces
{
    public interface IFileSystemBrowser : IUtility
    {
        string OpenDirectoryBrowser(string windowTitle);
        Task<string> OpenDirectoryBrowserAsync(string windowTitle);
        string OpenFileBrowser(string filter, string windowTitle);
        Task<string> OpenFileBrowserAsync(string filter, string windowTitle);
    }
}