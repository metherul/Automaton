using Automaton.Model.ModpackBase;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Automaton.Model.NexusApi
{
    public class NexusMod : NexusBase
    {
        private static List<Mod> QueuedModDownloads;

        public static void QueueModDownload(Mod mod)
        {
            QueuedModDownloads.Add(mod);
        }

        public static async Task DownloadModFile(Mod mod, IProgress<DownloadModFileProgress> progress)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://api.nexusmods.com");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("APIKEY", ApiKey);

                var response = await httpClient
                    .GetStringAsync(
                        $"/v1/games/{Instance.AutomatonInstance.ModpackHeader.TargetGame.ToLower()}/mods/{mod.NexusModId}/files/{mod.FileId}/download_link.json");
                var downloadPath = Path.Combine(Instance.AutomatonInstance.SourceLocation, mod.FileName);

                dynamic jsonObject = JArray.Parse(response)[0];

                var progressObject = new DownloadModFileProgress()
                {
                    Mod = mod,
                    DownloadLocation = downloadPath
                };

                using (var webClient = new WebClient())
                {
                    webClient.DownloadProgressChanged += (sender, e) =>
                    {
                        progressObject.CurrentDownloadPercentage =
                            (int) ((((double) e.BytesReceived / e.TotalBytesToReceive)) * 100);

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

        public static async Task<bool> VerifyUserPremium()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://api.nexusmods.com");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("APIKEY", ApiKey);

                var response = await httpClient
                    .GetStringAsync($"/v1/users/validate.json");

                dynamic jsonObject = JObject.Parse(response);
                var premiumStatus = (bool) jsonObject["is_premium"];

                return premiumStatus;

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