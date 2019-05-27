namespace Automaton.Model.Interfaces
{
    interface INxmRouter
    {
        bool IsClientConnected { get; set; }
        void ConnectServer();
        void ConnectClient();
        void SendMessageToServer<T>(T message);
    }
}
