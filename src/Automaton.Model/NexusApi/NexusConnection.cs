using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Automaton.Model.NexusApi
{
    public class NexusConnection : NexusBase
    {
        /// <summary>
        /// Creates a new websocket connection to Nexusmods.
        /// </summary>
        /// <param name="progressCallback"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public static async Task StartNewConnectionAsync(IProgress<NewConnectionProgress> progressCallback, string apiKey = "")
        {
            var progress = new NewConnectionProgress();
            var guid = Guid.NewGuid();

            if (!string.IsNullOrEmpty(apiKey))
            {
                ApiKey = apiKey;

                progress.CurrentMessage = "API key already detected, skipping websocket authentication.";
                progress.IsComplete = true;

                progressCallback.Report(progress);

                return;
            }

            // Initialize websocket connection
            WebSocket = new WebSocket("wss://sso.nexusmods.com");
            WebSocket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;

            // Create lambda for when the API key is recieved
            WebSocket.OnMessage += (sender, e) =>
            {
                if (e != null && !string.IsNullOrEmpty(e.Data))
                {
                    ApiKey = e.Data;

                    progress.CurrentMessage = "API key captured. Process successful.";
                    progress.IsComplete = true;

                    progressCallback.Report(progress);
                }
            };

            progress.CurrentMessage = "Attempting connection to 'sso.nexusmods.com'.";
            progressCallback.Report(progress);

            await Task.Factory.StartNew(() => WebSocket.Connect());

            progress.CurrentMessage = "Websocket connected. Sending API key to the Nexus.";
            progressCallback.Report(progress);

            await Task.Factory.StartNew(() => WebSocket.Send("{\"id\": \"" + guid + "\", \"appid\": \"Vortex\"}"));

            progress.CurrentMessage = "API key sent, opening web browser...";
            progressCallback.Report(progress);

            Process.Start($"https://www.nexusmods.com/sso?id={guid}");
        }
    }

    public class NewConnectionProgress
    {
        public string CurrentMessage;
        public bool IsComplete;
        public bool HasError;

        public NewConnectionProgress()
        {
            CurrentMessage = "";
            IsComplete = false;
            HasError = false;
        }
    }
}
