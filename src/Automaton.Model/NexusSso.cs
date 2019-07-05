using Automaton.Model.Interfaces;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Automaton.Model
{
    public class NexusSso : INexusSso
    {
        public delegate void GrabbedKey(string key);
        public event GrabbedKey GrabbedKeyEvent;

        public NexusSso New()
        {
            return this;
        }

        public async Task ConnectAndGrabKeyAsync()
        {
            await Task.Run(() =>
            {
                ConnectAndGrabKey();
            });
        }

        public void ConnectAndGrabKey()
        {
            var guid = Guid.NewGuid();
            var websocket = new WebSocket("wss://sso.nexusmods.com")
            {
                SslConfiguration =
                {
                    EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12
                }
            };

            websocket.OnMessage += Websocket_OnMessage;

            websocket.Connect();
            websocket.Send("{\"id\": \"" + guid + "\", \"appid\": \"Automaton\"}");

            Process.Start($"https://www.nexusmods.com/sso?id={guid}&application=Automaton");
        }

        private void Websocket_OnMessage(object sender, MessageEventArgs e)
        {
            var key = e.Data;

            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            GrabbedKeyEvent.Invoke(key);
        }
    }
}
