using System;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Automaton.Model.NexusApi
{
    public class ApiBase
    {
        private HttpClient HttpClient { get; set; }

        public string ApiKey { get; set; }
        public string GameName { get; set; }

        public bool Initialize(string gameName, string apiKey = "")
        {
            GameName = gameName;

            HttpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://api.nexusmods.com"),
                Timeout = TimeSpan.FromSeconds(5)
            };

            if (apiKey != string.Empty)
            {
                ApiKey = apiKey;
            }

            else
            {
                var guid = Guid.NewGuid();
                var websocket = new WebSocket("wss://sso.nexusmods.com")
                {
                    SslConfiguration = {EnabledSslProtocols = SslProtocols.Tls12}
                };


            }

            return false;
        }
    }
}
