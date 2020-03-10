using Gearbox.Modpacks.Base;
using Gearbox.NexusMods;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Gearbox.Repositories.NexusMods
{
    public class Repository : IRepository
    {
        public string RepositoryType { get; set; }
        
        public string ModId;
        public string FileId;

        public Domain Domain;

        public async Task DownloadFile(string targetLocation)
        {
            var nexus = new NexusApi();
            var downloadPaths = await nexus.GetDownloadPaths(Domain, Convert.ToInt32(ModId), Convert.ToInt32(FileId));

            await downloadPaths.IfSomeAsync(async (f) =>
            {
                var downloadPath = f.First().Uri;
                var client = new WebClient();
                await client.DownloadFileTaskAsync(downloadPath, targetLocation);
            });
        }

        public async Task DownloadFile(string nexusMods, string targetLocation)
        {

        }
    }
}