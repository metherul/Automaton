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
        /// <param name="modId"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static async Task DownloadModFile(string fileId, string modId, IProgress<DownloadModFileProgress> progress)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://api.nexusmods.com");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("APIKEY", ApiKey);

                var response = await httpClient.GetStringAsync($"/v1/games/{Instance.Automaton.ModpackHeader.TargetGame.ToLower()}/mods/{modId}/files/{fileId}/download_link");
                dynamic jsonObject = JObject.Parse(response.Replace("[", "").Replace("]", ""));

                var progressObject = new DownloadModFileProgress()
                {
                    FileId = fileId,
                    ModId = modId
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
                        progressObject.IsDownloadComplete = true;

                        progress.Report(progressObject);
                    };

                    var test = Instance.Automaton.SourceLocation;

                    webClient.DownloadFileAsync(new Uri(jsonObject.URI.Value), Path.Combine(Instance.Automaton.SourceLocation, "Test.7z"));
                }
            }
                    
        }
    }

    public class DownloadModFileProgress
    {
        public string FileId;
        public string ModId;

        public int CurrentDownloadPercentage;

        public bool IsDownloadComplete;
    }


}
