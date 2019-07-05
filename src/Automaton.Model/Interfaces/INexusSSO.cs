using System.Threading.Tasks;

namespace Automaton.Model.Interfaces
{
    public interface INexusSso : IModel
    {
        event NexusSso.GrabbedKey GrabbedKeyEvent;

        void ConnectAndGrabKey();
        Task ConnectAndGrabKeyAsync();
        NexusSso New();
    }
}