using CG.Web.MegaApiClient;
using System;
using System.Threading.Tasks;

namespace Gearbox.Repositories.MEGA
{
    public class Repository : IRepository
    {
        public string RepositoryType { get; set; }

        public string Url;
        
        public async Task DownloadFile(string targetLocation)
        {
            var client = new MegaApiClient();
            await client.LoginAnonymousAsync();
            await client.DownloadFileAsync(new Uri(Url), targetLocation, null);

            await client.LogoutAsync();
        }

        public async Task DownloadFile(string downloadPath, string targetLocation)
        {
            throw new System.NotImplementedException();
        }
    }
}