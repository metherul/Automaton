using System.Threading.Tasks;

namespace Gearbox.Repositories.Github
{
    public class Repository : IRepository
    {
        public string RepositoryType { get; set; }

        public string RepoUrl;
        public string Version;
        
        public async Task DownloadFile()
        {
            throw new System.NotImplementedException();
        }
    }
}