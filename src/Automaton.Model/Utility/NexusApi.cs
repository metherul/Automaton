using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using WebSocketSharp;
using System.Web;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace Automaton.Model.Utility
{
    public class NexusApi
    {
        private static string ApiKey;

        private static WebSocket WebSocket;

        public static async Task StartConnectionAsync()
        {
            // Initialize websocket connection
            WebSocket = new WebSocket("wss://sso.nexusmods.com");
            WebSocket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;

            WebSocket.OnMessage += (sender, e) =>
            {
                if (e != null && !string.IsNullOrEmpty(e.Data))
                {
                    ApiKey = e.Data;
                }
            };

            await Task.Factory.StartNew(() => WebSocket.Connect());

            var guid = Guid.NewGuid();

            await Task.Factory.StartNew(() => WebSocket.Send("{\"id\": \"" + guid + "\", \"appid\": \"Vortex\"}"));

            Process.Start($"https://www.nexusmods.com/sso?id={guid}");
        }

        public static async Task StartModDownload(string fileId)
        {

        }
    }

    public class StartNexusConnectionResult
    {
        public bool HasError;
        public string Result;
    }
}
