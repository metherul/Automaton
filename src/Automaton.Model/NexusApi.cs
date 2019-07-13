using Autofac;
using Automaton.Model.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Automaton.Model
{
    public class NexusApi : INexusApi
    {
        private readonly ILifetimeData _lifetimeData;

        private HttpClient _httpClient;

        private string _key;
        private int _remainingRequests;

        public bool IsLoggedIn { get; set; }
        public bool IsPremium { get; set; }

        public NexusApi(IComponentContext components)
        {
            _lifetimeData = components.Resolve<ILifetimeData>();
        }

        public async Task Init(string key)
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://api.nexusmods.com"),
                Timeout = TimeSpan.FromSeconds(10),
            };

            _httpClient.DefaultRequestHeaders.Add("User-Agent", _lifetimeData.UserAgent);
            _httpClient.DefaultRequestHeaders.Add("Application-Name", "Automaton");
            _httpClient.DefaultRequestHeaders.Add("Application-Version", "test");
            _httpClient.DefaultRequestHeaders.Add("APIKEY", key);
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            _key = key;
            IsPremium = await IsPremiumUser();
        }

        public async Task<bool> IsPremiumUser()
        {
            var validate = "/v1/users/validate.json";
            var result = await MakeGenericApiCall(validate);

            if (result == null)
            {
                IsLoggedIn = false;
                IsPremium = false;

                return false;
            }            

            var isPremium = (bool)JObject.Parse(result)["is_premium"];

            IsLoggedIn = true;
            IsPremium = isPremium;

            return isPremium;
        }

        public async Task<string> GetArchiveDownloadUrl(ExtendedArchive archive, string protocolParams = "")
        {
            var downloadLink = $"/v1/games/{archive.GameName}/mods/{archive.ModId}/files/{archive.FileId}/download_link.json{protocolParams}";
            var result = await MakeGenericApiCall(downloadLink);

            if (result == null)
            {
                return null;
            }

            return JArray.Parse(result)[0]["URI"].ToString();
        }        

        private async Task<string> MakeGenericApiCall(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }

                _remainingRequests = Convert.ToInt32(response.Headers.GetValues("X-RL-Daily-Remaining").ToList().First());

                var test = await response.Content.ReadAsStringAsync();

                return await response.Content.ReadAsStringAsync();
            }

            catch (Exception e)
            {
                return null;
            }
        }
    }
}
