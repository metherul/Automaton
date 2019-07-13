﻿using Automaton.Model.Interfaces;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Automaton.Model
{
    public class NexusSso : INexusSso
    {
        private WebSocket _websocket;

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
            _websocket = new WebSocket("wss://sso.nexusmods.com")
            {
                SslConfiguration =
                {
                    EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12
                }
            };

            _websocket.OnMessage += _websocket_OnMessage;

            _websocket.Connect();
            _websocket.Send("{\"id\": \"" + guid + "\", \"appid\": \"Automaton\"}");

            Process.Start($"https://www.nexusmods.com/sso?id={guid}&application=Automaton");
        }

        private void _websocket_OnMessage(object sender, MessageEventArgs e)
        {
            var key = e.Data;

            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            _websocket.Close();

            GrabbedKeyEvent.Invoke(key);
        }
    }
}