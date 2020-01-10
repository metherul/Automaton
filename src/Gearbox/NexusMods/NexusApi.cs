using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Gearbox.NexusMods
{
    public partial class NexusApi
    {
        private readonly HttpClient _baseHttpClient;

        private int _hourlyRequestsRemaining;
        private int _dailyRequestsRemaning;

        public NexusApi(string apiKey)
        {
            var platformType = Environment.Is64BitProcess ? "x64" : "x86";
            var headerString =
                $"Automaton/{Assembly.GetEntryAssembly().GetName().Version} ({Environment.OSVersion.VersionString}; " +
                $"{platformType}) {RuntimeInformation.FrameworkDescription}";
            
            _baseHttpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://api.nexusmods.com"),
                Timeout = TimeSpan.FromSeconds(5)
            };

            _baseHttpClient.DefaultRequestHeaders.Add("APIKEY", apiKey);
            _baseHttpClient.DefaultRequestHeaders.Add("User-Agent", headerString);
            _baseHttpClient.DefaultRequestHeaders.Add("Application-Version", "0.0.1");
            _baseHttpClient.DefaultRequestHeaders.Add("Application-Name", "Automaton");
            _baseHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string?> MakeGenericRequest(string address)
        {
            var response = await _baseHttpClient.GetAsync(address);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            
            _hourlyRequestsRemaining = Convert.ToInt32(response.Headers.GetValues("x-rl-hourly-remaining").First());
            _dailyRequestsRemaning = Convert.ToInt32(response.Headers.GetValues("x-rl-daily-remaining").First());

            return await response.Content.ReadAsStringAsync();
        }
    }
}