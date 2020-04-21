using System.Threading.Tasks;

namespace Gearbox.Scripts
{
    public interface IRunnableScript
    {
        Task Run(string args = "");
    }
}
