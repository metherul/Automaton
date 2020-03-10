using System;
using System.Net;
using System.Threading.Tasks;

namespace Gearbox.Repositories.Direct
{
    public class Repository : IRepository
    {
        public string RepositoryType { get; set; }

        public string Url;

        public async Task DownloadFile(string targetLocation)
        {
            var client = new WebClient();
            await client.DownloadFileTaskAsync(new Uri(Url), targetLocation);
        }

        public async Task DownloadFile(string downloadPath, string targetLocation)
        {
            var client = new WebClient();
            await client.DownloadFileTaskAsync(new Uri(downloadPath), targetLocation);
        }
    }
}