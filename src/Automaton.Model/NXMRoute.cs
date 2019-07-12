using Alphaleonis.Win32.Filesystem;
using Microsoft.Win32;
using NamedPipeWrapper;
using System;

namespace Automaton.Model
{
    public class NXMRoute
    {
        private NamedPipeServer<string> _server;
        private NamedPipeClient<string> _client;

        private readonly string _serverName = "automaton_ipc_server";

        public delegate void RecieveMessage(string messaage);
        public event RecieveMessage RecieveMessageEvent;

        /// <summary>
        /// Starts the server and waits for valid connections + messages
        /// </summary>
        public void StartServer()
        {
            _server = new NamedPipeServer<string>(_serverName);
            _server.ClientMessage += _server_ClientMessage;

            _server.Start();

            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Classes\nxm\shell\open\command", "", $"\"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Automaton.exe")}\" \"%1\"");
        }

        private void _server_ClientMessage(NamedPipeConnection<string, string> connection, string message)
        {
            RecieveMessageEvent.Invoke(message);
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
        /// This sends an object from the client to the server, if a client is connected
        /// </summary>
        /// <param name="message"></param>
        public void SendToServer(string message)
        {
            _client.PushMessage(message);
        }
    }
}
