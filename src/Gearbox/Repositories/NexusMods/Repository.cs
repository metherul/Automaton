using System.Threading.Tasks;

namespace Gearbox.Repositories.NexusMods
{
    public class Repository : IRepository
    {
        public string RepositoryType { get; set; }
        
        public string ModId;
        public string FileId;

        public async Task DownloadFile()
        {
            throw new System.NotImplementedException();
        }
    }
}