using System.Threading.Tasks;

namespace Gearbox.Repositories.MEGA
{
    public class Repository : IRepository
    {
        public string RepositoryType { get; set; }

        public string Url;
        
        public async Task DownloadFile(string targetLocation)
        {

        }

        public async Task DownloadFile(string downloadPath, string targetLocation)
        {
            throw new System.NotImplementedException();
        }
    }
}