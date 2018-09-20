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
    }
}