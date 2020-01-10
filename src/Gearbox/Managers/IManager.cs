using System.Threading.Tasks;
using Gearbox.Modpacks;

namespace Gearbox.Managers
{
    public interface IManager
    {
        Task InstallManager(string installDir);
        Task InstallMod(IMod mod);
    }
}