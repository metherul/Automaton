using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Threading.Tasks;
using Automaton.Model.NexusApi.Interfaces;
using Newtonsoft.Json.Linq;
using WebSocketSharp;

namespace Automaton.Model.NexusApi
{
    public class ApiBase : IApiBase
    {
        public delegate void HasLoggedIn();

        public event HasLoggedIn HasLoggedInEvent;

        private HttpClient HttpClient { get; set; }

        public string ApiKey { get; set; }

        public int RemainingDailyRequests { get; set; }

        protected bool IsPremium { get; set; }
        protected bool IsLoggedIn { get; set; }

        public async Task<bool> InitializeAsync(string apiKey = "")
        {
            return await Task.Factory.StartNew(() => Initialize(apiKey));
        }

        public bool Initialize(string apiKey = "")
        {
            HttpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://api.nexusmods.com"),
                Timeout = TimeSpan.FromSeconds(5)
            };

            if (apiKey != string.Empty)
            {
                ApiKey = apiKey;

                HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpClient.DefaultRequestHeaders.Add("APIKEY", ApiKey);

                // Get the premium status of the account
                var response = HttpClient.GetAsync("/v1/users/validate.json").Result;
                IsPremium = (bool)JObject.Parse(response.Content.ReadAsStringAsync().Result)["is_premium"];
                IsLoggedIn = true;

                RemainingDailyRequests = Convert.ToInt32(response.Headers.GetValues("X-RL-Daily-Remaining").ToList().First());

                HasLoggedInEvent.Invoke();
            }

            else
            {
                var guid = Guid.NewGuid();
                var websocket = new WebSocket("wss://sso.nexusmods.com")
                {
                    SslConfiguration = {EnabledSslProtocols = SslProtocols.Tls12}
                };


                websocket.OnMessage += (sender, args) =>
                {
                    if (args == null || string.IsNullOrEmpty(args.Data)) return;

                    ApiKey = args.Data;

                    HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpClient.DefaultRequestHeaders.Add("APIKEY", ApiKey);

                    // Get the premium status of the account
                    var response = HttpClient.GetAsync("/v1/users/validate.json").Result;
                    IsPremium = (bool)JObject.Parse(response.Content.ReadAsStringAsync().Result)["is_premium"];
                    IsLoggedIn = true;

                    RemainingDailyRequests = Convert.ToInt32(response.Headers.GetValues("X-RL-Daily-Remaining").ToList().First());

                    HasLoggedInEvent.Invoke();
                };

                websocket.Connect();
                websocket.Send("{\"id\": \"" + guid + "\", \"appid\": \"Automaton\"}");

                Process.Start($"https://www.nexusmods.com/sso?id={guid}");
            }

            return false;
        }

        public bool IsUserLoggedIn()
        {
            return IsLoggedIn;
        }
    }
}
