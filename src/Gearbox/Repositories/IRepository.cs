using System.Threading.Tasks;

namespace Gearbox.Repositories
{
    public interface IRepository
    {
        string RepositoryType { get; set; }
        Task DownloadFile();
    }
}