using System.Threading.Tasks;
using Automaton.Model.Interfaces;

namespace Automaton.Model.Modpack.Interfaces
{
    public interface IModpackRead : IModel
    {
        bool LoadModpack(string modpackPath);
        Task<bool> LoadModpackAsync(string modpackPath);
    }
}