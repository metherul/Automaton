using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using Automaton.Utils;
using System.Threading;
using System.IO;

namespace Hephaestus.Nexus
{
    public class NexusClient
    {
        private string apiKey;
        private HttpClient _baseHttpClient;

        public NexusClient(string api_key)
        {
            this.apiKey = api_key;
            _baseHttpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://api.nexusmods.com"),
                Timeout = TimeSpan.FromSeconds(10),
            };

            var platformType = Environment.Is64BitOperatingSystem ? "x64" : "x86";
            var headerString = $"Automaton/{Assembly.GetEntryAssembly().GetName().Version} ({Environment.OSVersion.VersionString}; {platformType}) {RuntimeInformation.FrameworkDescription}";

            _baseHttpClient.DefaultRequestHeaders.Add("User-Agent", headerString);
            _baseHttpClient.DefaultRequestHeaders.Add("apikey", api_key);
            _baseHttpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _baseHttpClient.DefaultRequestHeaders.Add("Application-Name", "Automaton");
            _baseHttpClient.DefaultRequestHeaders.Add("Application-Version", $"{Assembly.GetEntryAssembly().GetName().Version}");
        }

        public List<MD5SearchResult> MD5Search(string game, string md5)
        {
            while (true)
            {
                var uri = string.Format("/v1/games/{0}/mods/md5_search/{1}.json", game, md5);
                Task<Stream> http_request;
                try
                {
                    http_request = _baseHttpClient.GetStreamAsync(uri);
                    http_request.Wait();
                }
                catch (TaskCanceledException ex)
                {
                    Log.Warn("Nexus told us to back off a bit, waiting for a few seconds");
                    Thread.Sleep(3000);
                    continue;
                }
                catch (Exception e)
                {
                    Log.Warn("Failed to find MD5 {0}", md5);
                    return null;
                }

                var result = http_request.Result;

                var json = Utils.LoadJson<List<MD5SearchResult>>(result);

                return json;
            }

        }
    }
}
