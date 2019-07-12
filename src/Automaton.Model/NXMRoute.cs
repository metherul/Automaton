using Alphaleonis.Win32.Filesystem;
using Automaton.Model.Interfaces;
using Microsoft.Win32;
using NamedPipeWrapper;
using System;

namespace Automaton.Model
{
    public class NXMRoute : INXMRoute
    {
        private NamedPipeServer<string> _server;
        private NamedPipeClient<string> _client;

        private readonly string _serverName = "automaton_ipc_server";

        public delegate void RecieveMessage(string message);
        public event RecieveMessage RecieveMessageEvent;

        private void _server_ClientMessage(NamedPipeConnection<string, string> connection, string message)
        {
            RecieveMessageEvent.Invoke(message);
        }

        /// <summary>
        /// Starts the server and waits for valid connections + messages
        /// </summary>
        public void StartServer()
        {
            _server = new NamedPipeServer<string>(_serverName);
            _server.ClientMessage += _server_ClientMessage;

            _server.Start();

            // This is temp
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Classes\nxm\shell\open\command", "", $"\"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Automaton.exe")}\" \"%1\"");
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Classes\nxm", "", $"URL:NXM Protocol");
        }

        /// <summary>
        /// Connects to the default pipe server, if one exists
        /// </summary>
        public void ConnectClient()
        {
            _client = new NamedPipeClient<string>(_serverName);
            _client.Start();

            _client.WaitForConnection();
        }

        /// <summary>
        /// This sends an object from the client to the server, if a client is connected.
        /// </summary>
        /// <param name="message"></param>
        public void SendToServer(string message)
        {
            _client.PushMessage(message);
        }

        public PipedData ToPipedData(string nxmString)
        {
            var pipedData = new PipedData();
            var splitString = nxmString.Split('/');

            pipedData.NxmString = nxmString;
            pipedData.Game = splitString[2];
            pipedData.ModId = splitString[4];
            pipedData.FileId = splitString[6].Split('?')[0];

            if (pipedData.Game == "FalloutNV")
            {
                pipedData.Game = "newvegas";
            }

            pipedData.AuthenticationParams = "?" + splitString[6].Split('?')[1];

            return pipedData;
        }
    }

    public class PipedData
    {
        public string NxmString { get; set; }
        public string Game { get; set; }
        public string ModId { get; set; }
        public string FileId { get; set; }

        public string AuthenticationParams { get; set; }
    }
}
