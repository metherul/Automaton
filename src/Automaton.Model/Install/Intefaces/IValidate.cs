using System.Collections.Generic;
using System.Threading.Tasks;
using Automaton.Model.Interfaces;
using Automaton.Model.Modpack.Base;

namespace Automaton.Model.Install.Intefaces
{
    public interface IValidate : IModel
    {
        Task<List<Mod>> GetMissingModsAsync(params string[] directoriesToScan);
        List<Mod> GetMissingMods(params string[] directoriesToScan);
    }
}