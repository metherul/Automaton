using System;
using System.IO;
using NamedPipeWrapper;

namespace Automaton.Model.Handles
{
    public class NamedPipesHandler
    {
        public static NamedPipeServer<string> PipeServer { get; set; }

        private static IProgress<string> ResultNotifier { get; set; }

        public static StreamReader PipeReader { get; set; }

        public static void InitializeServer(IProgress<string> resultNotifier)
        {
            ResultNotifier = resultNotifier;

            PipeServer = new NamedPipeServer<string>("Automaton_PIPE");
            PipeServer.ClientMessage += PipeServer_ClientMessage;

            PipeServer.Start();
        }

        private static void PipeServer_ClientMessage(NamedPipeConnection<string, string> connection, string message)
        {
            RouteMessage(message);
        }

        public static void RouteMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ResultNotifier.Report(message);
            }
        }
    }

    enum PipeUpdate
    {
        MessageRecievedUpdate
    }
}
