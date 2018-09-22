using NamedPipeWrapper;
using System;

namespace Automaton.Model.Utility
{
    public class NamedPipes
    {
        private NamedPipeServer<string> PipeServer;

        public NamedPipes(IProgress<string> progress)
        {
            PipeServer = new NamedPipeServer<string>("Automaton_PIPE");
            PipeServer.ClientMessage += (connection, message) =>
            {
                if (!string.IsNullOrEmpty(message))
                {
                    progress.Report(message);
                }
            };
        }

        public void StartServer()
        {
            PipeServer.Start();
        }

        public static void SendMessage(string message)
        {
            var client = new NamedPipeClient<string>("Automaton_PIPE");

            client.Start();
            client.WaitForConnection(4000);
            client.PushMessage(message);
        }
    }
}