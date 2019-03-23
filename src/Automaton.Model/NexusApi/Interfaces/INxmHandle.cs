using System;
using Automaton.Model.Interfaces;

namespace Automaton.Model.NexusApi.Interfaces
{
    public interface INxmHandle : IService
    {
        event EventHandler<PipedData> RecievedPipedDataEvent;
        void ConnectClient();
        void SendClientMessage(PipedData message);
        void StartServer();
        void ReceieveClientMessage(PipedData message);
    }
}