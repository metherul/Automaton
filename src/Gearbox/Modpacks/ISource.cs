using System.Threading.Tasks;
using Gearbox.Repositories;

namespace Gearbox.Modpacks
{
    public interface ISource
    {
        string Name { get; }
        string Author { get; }
        
        string FilePath { get; }
        
        IRepository Repository { get; }

        Task DownloadFile(string downloadPath);
        
        Task Validate();

        Task Register();
    }
}