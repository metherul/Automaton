using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Automaton.Model.NexusApi
{
    public class NexusMod : NexusBase
    {
        public static async Task DownloadModFile(string fileId, IProgress<DownloadModFileProgress> progress)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://api.nexusmods.com");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("APIKEY", ApiKey);

                var response = await httpClient.GetStringAsync($"/v1/games/skyrim/mods/{fileId}/");
            }
                    
        }
    }

    public class DownloadModFileProgress
    {

    }


}
