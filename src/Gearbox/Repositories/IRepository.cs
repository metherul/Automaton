using System.Threading.Tasks;

namespace Gearbox.Repositories
{
    public interface IRepository
    {
        string RepositoryType { get; set; }
        Task DownloadFile(string downloadPath, string targetLocation);
        Task DownloadFile(string targetLocation);
    }
}