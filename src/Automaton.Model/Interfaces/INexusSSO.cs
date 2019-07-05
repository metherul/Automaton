using System.Threading.Tasks;

namespace Automaton.Model.Interfaces
{
    public interface INexusSso
    {
        event NexusSso.GrabbedKey GrabbedKeyEvent;

        Task ConnectAndGrabKey();
        NexusSso New();
    }
}