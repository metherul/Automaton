using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using Automaton.Model.NexusApi.Interfaces;
using Newtonsoft.Json.Linq;

namespace Automaton.Model.NexusApi
{
    public class ApiEndpoints : IApiEndpoints
    {
        private readonly IApiBase _apiBase;
        private readonly HttpClient _baseHttpClient;

        public ApiEndpoints(IComponentContext components)
        {
            _apiBase = components.Resolve<IApiBase>();

            _baseHttpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://api.nexusmods.com"),
                Timeout = TimeSpan.FromSeconds(10)
            };

            _baseHttpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string> GenerateModDownloadLinkAsync(string gameName, string modId, string fileId)
        {
            var url = $"/v1/games/{gameName}/mods/{modId}/files/{fileId}/download_link";

            var apiResult = await MakeGenericApiCall(url);

            return JArray.Parse(apiResult)[0]["URI"].ToString();
        }

        public async Task<string> GenerateModDownloadLinkAsync(PipedData pipedData)
        {
            var url = $"/v1/games/{pipedData.Game}/mods/{pipedData.ModId}/files/{pipedData.FileId}/download_link" +
                      pipedData.AuthenticationParams;
            var apiResult = await MakeGenericApiCall(url);

            return JArray.Parse(apiResult)[0]["URI"].ToString();
        }

        private async Task<string> MakeGenericApiCall(string url)
        {
            if (_apiBase.ApiKey == string.Empty)
            {
                // Throw exception here.
                return null;
            }

            try
            {
                _baseHttpClient.DefaultRequestHeaders.Add("APIKEY", _apiBase.ApiKey);

                var response = await _baseHttpClient.GetAsync(url);

                _apiBase.RemainingDailyRequests =
                    Convert.ToInt32(response.Headers.GetValues("X-RL-Daily-Remaining").ToList().First());

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);   
                throw;
            }
        }
    }
}
