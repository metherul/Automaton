using System.Threading.Tasks;

namespace Gearbox.Repositories.Direct
{
    public class Repository : IRepository
    {
        public string RepositoryType { get; set; }

        public string Url;
        
        public async Task DownloadFile()
        {
            throw new System.NotImplementedException();
        }
    }
}