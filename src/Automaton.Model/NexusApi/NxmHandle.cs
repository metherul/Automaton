using System;
using System.Windows.Forms;
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

            _server.ClientMessage += (connection, message) => ReceieveClientMessage(message);

            _server.Start();
        }

        public void ConnectClient()
        {


            _client = new NamedPipeClient<PipedData>(ServerName);
            _client.Start();

            _client.WaitForConnection();
        }

        public void SendClientMessage(PipedData message)
        {
            try
            {
                _client.PushMessage(message);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void ReceieveClientMessage(PipedData message)
        {
            RecievedPipedDataEvent(this, message);
        }
    }
}
