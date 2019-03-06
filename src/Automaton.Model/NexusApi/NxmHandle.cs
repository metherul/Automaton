using System;
using Automaton.Model.NexusApi.Interfaces;
using NamedPipeWrapper;

namespace Automaton.Model.NexusApi
{
    public class NxmHandle : INxmHandle
    {
        public event EventHandler<PipedData> RecievedPipedDataEvent;

        private const string ServerName = "automaton_ipc_server";

        private NamedPipeServer<PipedData> _server;
        private NamedPipeClient<PipedData> _client;

        public void StartServer()
        {
            _server = new NamedPipeServer<PipedData>(ServerName);

            _server.ClientConnected += connection => ConnectClient();
            _server.ClientMessage += (connection, message) => ServerCallback(message);

            _server.Start();
        }

        public void ConnectClient()
        {
            _client = new NamedPipeClient<PipedData>(ServerName);
            _client.Start();
        }

        public void SendClientMessage(PipedData message)
        {
            _client.PushMessage(message);
        }

        public void ReceieveClientMessage(PipedData message)
        {
            RecievedPipedDataEvent(this, message);
        }
    }
}
