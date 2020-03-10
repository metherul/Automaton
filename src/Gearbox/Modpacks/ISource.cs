using System.Threading.Tasks;
using Gearbox.Repositories;

namespace Gearbox.Modpacks
{
    public interface ISource
    {
        string Name { get; }
        string Author { get; }
        
        string FilePath { get; }
        
        Option<IRepository> Repository { get; }

        Task DownloadFile(string downloadPath);
        
        Task Validate();

        void Register(string archivePath);

        Task<string> FindMatchInDir(string dir);
    }
}