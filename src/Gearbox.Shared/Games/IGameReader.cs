using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gearbox.Shared.Games
{
    public interface IGameReader
    {
        bool IsGameExe(string exe);
        Task<List<string>> GetGameFiles();
    }
}
