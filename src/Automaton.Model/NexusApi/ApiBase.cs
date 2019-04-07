using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Threading.Tasks;
using Autofac;
using Automaton.Model.Interfaces;
using Automaton.Model.NexusApi.Interfaces;
using Newtonsoft.Json.Linq;
using WebSocketSharp;

namespace Automaton.Model.NexusApi
{
    public class ApiBase : IApiBase
    {
        private readonly ILogger _logger;

        public delegate void HasLoggedIn();

        public event HasLoggedIn HasLoggedInEvent;

        private HttpClient HttpClient { get; set; }

        public string ApiKey { get; set; }

        public int RemainingDailyRequests { get; set; }

        protected bool IsPremium { get; set; }
        protected bool IsLoggedIn { get; set; }

        public ApiBase(IComponentContext components)
        {
            _logger = components.Resolve<ILogger>();
        }

        public async Task<bool> InitializeAsync(string apiKey = "")
        {
            return await Task.Factory.StartNew(() => Initialize(apiKey));
        }

        public bool Initialize(string apiKey = "")
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            _logger.WriteLine("Initializing API base");

            HttpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://api.nexusmods.com"),
                Timeout = TimeSpan.FromSeconds(5)
            };

            if (apiKey != string.Empty)
            {
                _logger.WriteLine("Apikey not empty");

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
                _logger.WriteLine("Apikey empty");

                var guid = Guid.NewGuid();
                var websocket = new WebSocket("wss://sso.nexusmods.com")
                {
                    SslConfiguration = {EnabledSslProtocols = SslProtocols.Tls12}
                };


                websocket.OnMessage += (sender, args) =>
                {
                    _logger.WriteLine("Key captured");

                    if (args == null || string.IsNullOrEmpty(args.Data)) return;

                    ApiKey = args.Data;

                    HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpClient.DefaultRequestHeaders.Add("APIKEY", ApiKey);

                    // Get the premium status of the account
                    var response = HttpClient.GetAsync("/v1/users/validate.json").Result;
                    IsPremium = (bool)JObject.Parse(response.Content.ReadAsStringAsync().Result)["is_premium"];
                    IsLoggedIn = true;

                    RemainingDailyRequests = Convert.ToInt32(response.Headers.GetValues("X-RL-Daily-Remaining").ToList().First());

                    _logger.WriteLine($"User premium status: {IsPremium}");
                    _logger.WriteLine("User is logged in");

                    HasLoggedInEvent.Invoke();

                    websocket.Close();
                };

                websocket.Connect();
                websocket.Send("{\"id\": \"" + guid + "\", \"appid\": \"Automaton\"}");

                Process.Start($"https://www.nexusmods.com/sso?id={guid}&application=\"Automaton\"");
            }

            return false;
        }

        public bool IsUserLoggedIn()
        {
            return IsLoggedIn;
        }

        public bool IsUserPremium()
        {
            return IsPremium;
        }
    }
}
