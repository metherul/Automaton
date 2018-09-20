using Automaton.Model.ModpackBase;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Automaton.Model.NexusApi
{
    public class NexusMod : NexusBase
    {
        /// <summary>
        /// Downloads mod file with matching fileId and modId parameters.
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="fileId"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static async Task DownloadModFile(Mod mod, string fileId, IProgress<DownloadModFileProgress> progress)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://api.nexusmods.com");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("APIKEY", ApiKey);

                var response = await httpClient
                    .GetStringAsync($"/v1/games/{Instance.AutomatonInstance.ModpackHeader.TargetGame.ToLower()}/mods/{mod.NexusModId}/files/{fileId}/download_link");
                var downloadPath = Path.Combine(Instance.AutomatonInstance.SourceLocation, mod.FileName);

                dynamic jsonObject = JObject.Parse(response.Replace("[", "").Replace("]", ""));

                var progressObject = new DownloadModFileProgress()
                {
                    Mod = mod,
                    DownloadLocation = downloadPath
                };

                using (var webClient = new WebClient())
                {
                    webClient.DownloadProgressChanged += (sender, e) =>
                    {
                        progressObject.CurrentDownloadPercentage = (int)((((double)e.BytesReceived / e.TotalBytesToReceive)) * 100);

                        progress.Report(progressObject);
                    };

                    webClient.DownloadFileCompleted += (sender, e) =>
                    {
                        progressObject.CurrentDownloadPercentage = 100;
                        progressObject.IsDownloadComplete = true;

                        progress.Report(progressObject);
                    };

                    webClient.DownloadFileAsync(new Uri(jsonObject.URI.Value), downloadPath);
                }
            }
        }
    }

    public class DownloadModFileProgress
    {
        public Mod Mod;
        public int CurrentDownloadPercentage;
        public bool IsDownloadComplete;

        public string DownloadLocation;
    }
}