using System;
using System.IO;
using System.Windows.Forms;
using Automaton.Model.NexusApi.Interfaces;
using Microsoft.Win32;
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

            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Classes\nxm\shell\open\command", "", $"\"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Automaton.exe")}\" \"%1\"");
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
