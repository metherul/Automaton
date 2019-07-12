namespace Automaton.Model.Interfaces
{
    public interface INXMRoute : ISingleton
    {
        event NXMRoute.RecieveMessage RecieveMessageEvent;

        void ConnectClient();
        void SendToServer(string message);
        void StartServer();
        PipedData ToPipedData(string nxmString);
    }
}